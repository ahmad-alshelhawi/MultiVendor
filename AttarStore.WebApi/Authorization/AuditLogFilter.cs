// WebApi/Filters/AuditLogFilter.cs
using AttarStore.Domain.Attributes;
using AttarStore.Domain.Entities.Auth;
using AttarStore.Domain.Interfaces;
using AttarStore.Services.Data;         // AppDbContext
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace AttarStore.WebApi.Filters
{
    public class AuditLogFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionDescriptor is not ControllerActionDescriptor cad)
            {
                await next();
                return;
            }

            var method = cad.MethodInfo;
            var ctrl = cad.ControllerTypeInfo;

            // Skip if [NoAudit]
            if (method.IsDefined(typeof(NoAuditAttribute), true) ||
                ctrl.IsDefined(typeof(NoAuditAttribute), true))
            {
                await next();
                return;
            }

            // Skip if [NoAuditForRoles] and user in role
            var noAuditFor = method.GetCustomAttribute<NoAuditForRolesAttribute>(true)
                           ?? ctrl.GetCustomAttribute<NoAuditForRolesAttribute>(true);
            if (noAuditFor != null &&
                noAuditFor.Roles.Any(r => context.HttpContext.User.IsInRole(r)))
            {
                await next();
                return;
            }

            // Get raw claims
            var userClaims = context.HttpContext.User;
            var idClaim = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = userClaims.FindFirst(ClaimTypes.Role)?.Value ?? "";
            if (!int.TryParse(idClaim, out var actorId))
            {
                // anonymous
                await next();
                return;
            }

            // Determine actor type
            string actorType = roleClaim switch
            {
                Roles.Admin => "Admin",
                Roles.Client => "Client",
                _ => "User"
            };

            // Lookup real name from DB
            var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
            string actorName = actorType switch
            {
                "Admin" => await db.Admins.Where(a => a.Id == actorId)
                                           .Select(a => a.Name).FirstOrDefaultAsync() ?? "",
                "Client" => await db.Clients.Where(c => c.Id == actorId)
                                            .Select(c => c.Name).FirstOrDefaultAsync() ?? "",
                _ => await db.Users.Where(u => u.Id == actorId)
                                          .Select(u => u.Name).FirstOrDefaultAsync() ?? ""
            };

            // Capture any route arg named {something}Id
            int? entityIdArg = null;
            foreach (var kv in context.ActionArguments)
                if (kv.Key.EndsWith("Id", StringComparison.OrdinalIgnoreCase)
                    && int.TryParse(kv.Value?.ToString(), out var v))
                {
                    entityIdArg = v;
                    break;
                }

            // Run the action
            var executedCtx = await next();

            // Try to pull created Id out of result if not from route
            int? finalEntityId = entityIdArg;
            if (finalEntityId == null && executedCtx.Result is ObjectResult objResult)
            {
                var val = objResult.Value;
                var idProp = val?.GetType().GetProperty("Id");
                if (idProp != null)
                    finalEntityId = (int?)idProp.GetValue(val, null);
            }

            // Now lookup the entity name if we have an Id
            string? entityName = null;
            if (finalEntityId.HasValue)
            {
                switch (cad.ControllerName)
                {
                    case "Category":
                        entityName = await db.Categories
                                             .Where(c => c.Id == finalEntityId.Value)
                                             .Select(c => c.Name)
                                             .FirstOrDefaultAsync();
                        break;
                    case "Product":
                        entityName = await db.Products
                                             .Where(p => p.Id == finalEntityId.Value)
                                             .Select(p => p.Name)
                                             .FirstOrDefaultAsync();
                        break;
                        // add cases for Subcategory, Vendor, Order, etc.
                }
            }

            // Build details
            var details = new
            {
                Controller = cad.ControllerName,
                Action = cad.ActionName,
                Args = context.ActionArguments,
                Status = context.HttpContext.Response.StatusCode
            };

            // Save audit
            var repo = context.HttpContext.RequestServices
                            .GetRequiredService<IAuditLogRepository>();

            var entry = new AuditLog
            {
                ActorId = actorId,
                ActorType = actorType,
                ActorName = actorName,
                ActorRole = roleClaim,
                Action = $"{cad.ControllerName}.{cad.ActionName}",
                EntityType = cad.ControllerName,
                EntityId = finalEntityId,
                EntityName = entityName,
                Timestamp = DateTime.UtcNow,
                Details = JsonSerializer.Serialize(details)
            };

            await repo.AddAsync(entry);
        }
    }
}
