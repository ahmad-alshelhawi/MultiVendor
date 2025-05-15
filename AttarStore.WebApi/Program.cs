using AttarStore.Api.Profiles;
using AttarStore.Api.Utils;
using AttarStore.Application.MappingProfiles;
using AttarStore.Application.Settings;            // ← ADDED (for EmailSettings)
using AttarStore.Domain.Interfaces;
using AttarStore.Domain.Interfaces.Catalog;
using AttarStore.Domain.Interfaces.Shopping;
using AttarStore.Infrastructure.Events;
using AttarStore.Infrastructure.Hubs;
using AttarStore.Infrastructure.Interfaces;
using AttarStore.Infrastructure.Repositories;
using AttarStore.Infrastructure.Repositories.Catalog;
using AttarStore.Infrastructure.Repositories.Shopping;
using AttarStore.Infrastructure.Services;         // ← ADDED (for EmailSender)
using AttarStore.Infrastructure.Services;         // for IEmailSender alias
using AttarStore.Services;
using AttarStore.Services.Data;
using AttarStore.Services.Interfaces.Catalog;
using AttarStore.Services.Repositories;
using AttarStore.Services.Repositories.Catalog;
using AttarStore.WebApi.Authorization;
using AttarStore.WebApi.Filters;
using AttarStore.WebApi.Providers;
using MediatR;                                     // ← ADDED (for IMediator)
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using IEmailSender = AttarStore.Infrastructure.Services.IEmailSender;

var builder = WebApplication.CreateBuilder(args);

// ─── EF Core DbContext ─────────────────────────────────────────────────────
builder.Services.AddDbContextPool<AppDbContext>(options =>
{
    var connName = builder.Environment.IsDevelopment() ? "Dev" : "Main";
    options
        .UseSqlServer(builder.Configuration.GetConnectionString(connName))
        .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
});

// ─── Email Settings & Service ────────────────────────────────────────────────
builder.Services.Configure<EmailSettings>(                                       // ← ADDED
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();                       // ← ADDED

// ─── Cookie Policy ─────────────────────────────────────────────────────────
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});

// ─── CORS ───────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AttarStorePolicy", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());

    options.AddPolicy("SignalR", policy =>
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(_ => true));
});

// ─── Authentication (JWT Bearer) ────────────────────────────────────────────
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var cfg = builder.Configuration;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = cfg["JWT:Issuer"],
        ValidAudience = cfg["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
                                     Encoding.UTF8.GetBytes(cfg["JWT:Key"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            if (ctx.Request.Cookies.TryGetValue("accessToken", out var token))
                ctx.Token = token;
            return Task.CompletedTask;
        }
    };
});

// ─── Authorization (Permission‐based) ───────────────────────────────────────
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddAuthorization(options =>
{
    {
        // Category
        options.AddPolicy("Category.Create", policy => policy.Requirements.Add(new PermissionRequirement("Category.Create")));
        options.AddPolicy("Category.Read", policy => policy.Requirements.Add(new PermissionRequirement("Category.Read")));
        options.AddPolicy("Category.Update", policy => policy.Requirements.Add(new PermissionRequirement("Category.Update")));
        options.AddPolicy("Category.Delete", policy => policy.Requirements.Add(new PermissionRequirement("Category.Delete")));

        // Category Request

        options.AddPolicy("CategoryRequest.Create", policy =>
         policy.Requirements.Add(new PermissionRequirement("CategoryRequest.Create")));

        options.AddPolicy("CategoryRequest.ReadOwn", policy =>
            policy.Requirements.Add(new PermissionRequirement("CategoryRequest.ReadOwn")));

        options.AddPolicy("CategoryRequest.ReadAll", policy =>
            policy.Requirements.Add(new PermissionRequirement("CategoryRequest.ReadAll")));

        options.AddPolicy("CategoryRequest.Update", policy =>
            policy.Requirements.Add(new PermissionRequirement("CategoryRequest.Update")));


        options.AddPolicy("VendorUser.Read", policy => policy.Requirements.Add(
          new PermissionRequirement("VendorUser.Read")
        )
      );
        options.AddPolicy("VendorUser.Create", policy => policy.Requirements.Add(
          new PermissionRequirement("VendorUser.Create")
        )
      );
        options.AddPolicy("VendorUser.Update", policy => policy.Requirements.Add(
          new PermissionRequirement("VendorUser.Update")
        )
      );

        // Order
        options.AddPolicy("Order.Create", policy => policy.Requirements.Add(new PermissionRequirement("Order.Create")));
        options.AddPolicy("Order.ReadAll", policy => policy.Requirements.Add(new PermissionRequirement("Order.ReadAll")));
        options.AddPolicy("Order.ReadOwn", policy => policy.Requirements.Add(new PermissionRequirement("Order.ReadOwn")));
        options.AddPolicy("Order.Update", policy => policy.Requirements.Add(new PermissionRequirement("Order.Update")));
        options.AddPolicy("Order.Delete", policy => policy.Requirements.Add(new PermissionRequirement("Order.Delete")));

        // Product
        options.AddPolicy("Product.Create", policy => policy.Requirements.Add(new PermissionRequirement("Product.Create")));
        options.AddPolicy("Product.Read", policy => policy.Requirements.Add(new PermissionRequirement("Product.Read")));
        options.AddPolicy("Product.Update", policy => policy.Requirements.Add(new PermissionRequirement("Product.Update")));
        options.AddPolicy("Product.Delete", policy => policy.Requirements.Add(new PermissionRequirement("Product.Delete")));

        // ─── NEW Permission‐management policies ───────────────────────────────────
        options.AddPolicy("Permission.Read", policy => policy.Requirements.Add(new PermissionRequirement("Permission.Read")));
        options.AddPolicy("Permission.Update", policy => policy.Requirements.Add(new PermissionRequirement("Permission.Update")));
        options.AddPolicy("Permission.Delete", policy => policy.Requirements.Add(new PermissionRequirement("Permission.Delete")));
        options.AddPolicy("Permission.Create", policy => policy.Requirements.Add(new PermissionRequirement("Permission.Create")));

        // Users-Vendors
        options.AddPolicy("VendorUser.Read", policy => policy.Requirements.Add(new PermissionRequirement("User.ReadOwn")));
        options.AddPolicy("VendorUser.Create", policy => policy.Requirements.Add(new PermissionRequirement("User.Create")));

        // If you want to distinguish admin-employees:
        options.AddPolicy("AdminUser.Read", policy => policy.Requirements.Add(new PermissionRequirement("User.ReadOwn")));
        options.AddPolicy("AdminUser.Create", policy => policy.Requirements.Add(new PermissionRequirement("User.Create")));
        // Vendor
        options.AddPolicy("Vendor.Create", policy => policy.Requirements.Add(new PermissionRequirement("Vendor.Create")));
        options.AddPolicy("Vendor.Read", policy => policy.Requirements.Add(new PermissionRequirement("Vendor.Read")));
        options.AddPolicy("Vendor.Update", policy => policy.Requirements.Add(new PermissionRequirement("Vendor.Update")));
        options.AddPolicy("Vendor.Delete", policy => policy.Requirements.Add(new PermissionRequirement("Vendor.Delete")));


        options.AddPolicy("CategoryRequest.Create", policy =>
            policy.Requirements.Add(new PermissionRequirement("CategoryRequest.Create")));
        options.AddPolicy("CategoryRequest.ReadOwn", policy =>
            policy.Requirements.Add(new PermissionRequirement("CategoryRequest.ReadOwn")));
        options.AddPolicy("CategoryRequest.ReadAll", policy =>
            policy.Requirements.Add(new PermissionRequirement("CategoryRequest.ReadAll")));
        options.AddPolicy("CategoryRequest.Update", policy =>
            policy.Requirements.Add(new PermissionRequirement("CategoryRequest.Update")));


        options.AddPolicy("AuditLog.ReadAll", policy =>
           policy.RequireRole("Admin"));              // only Admin role

        options.AddPolicy("AuditLog.ReadOwn", policy =>
            policy.RequireAuthenticatedUser());        // any logged-in user

        // Notification‐send policy
        options.AddPolicy("Notification.Send",
            policy => policy.RequireRole("Admin", "VendorAdmin"));
    }
});

// ─── AutoMapper, Repos & Services ───────────────────────────────────────────
builder.Services.AddAutoMapper(
    typeof(AdminProfile).Assembly,
    typeof(ProductProfile).Assembly,
    typeof(AuditLogProfile).Assembly,
    typeof(UserProfile).Assembly,
    typeof(CategoryRequestProfile).Assembly,
    typeof(NotificationMappingProfile).Assembly
);

// Core repositories
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();

// Catalog repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ISubcategoryRepository, SubcategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IVariantOptionRepository, VariantOptionRepository>();
builder.Services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<IProductVariantImageRepository, ProductVariantImageRepository>();
builder.Services.AddScoped<ICategoryRequestRepository, CategoryRequestRepository>();

// Shopping repositories
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Token & email
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<TokenService>();
//builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<IVendorService, VendorService>();

// Permission management
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

// Audit log
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextAccessor, HttpContextUserAccessor>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<AuditLogFilter>();

// SignalR
builder.Services.AddSignalR();

// Notifications
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

// ─── MediatR Setup ───────────────────────────────────────────────────────────
// ✅ use the Action‐based overload and register your event/handler assembly
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(OrderPlacedEvent).Assembly);
});




// ─── Controllers & Swagger ─────────────────────────────────────────────────
builder.Services.AddControllers(options =>
{
    options.Filters.Add<AuditLogFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "W Motors", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ─── Middleware Pipeline ───────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCookiePolicy();
app.UseCors("AttarStorePolicy");
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// SignalR & Notifications
app.UseCors("SignalR");
app.MapHub<NotificationHub>("/hubs/notifications");

app.MapControllers();
app.ApplyMigrations();
app.Run();