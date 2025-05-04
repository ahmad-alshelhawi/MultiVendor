using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AttarStore.Api.Utils;
using AttarStore.Domain.Interfaces;
using AttarStore.Domain.Interfaces.Catalog;
using AttarStore.Domain.Interfaces.Shopping;
using AttarStore.Infrastructure.Repositories.Catalog;
using AttarStore.Infrastructure.Repositories.Shopping;
using AttarStore.Infrastructure.Services;
using AttarStore.Services;
using AttarStore.Services.Data;
using AttarStore.Services.Interfaces.Catalog;
using AttarStore.Services.Repositories;
using AttarStore.Services.Repositories.Catalog;
using AttarStore.WebApi.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using IEmailSender = AttarStore.Infrastructure.Services.IEmailSender;

var builder = WebApplication.CreateBuilder(args);

// ─── Cookie Policy ─────────────────────────────────────────────────────────
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});

// Ensure SameSite=None cookies are honored
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// ─── CORS ───────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AttarStorePolicy", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
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
    // Category
    options.AddPolicy("Category.Create", policy => policy.Requirements.Add(new PermissionRequirement("Category.Create")));
    options.AddPolicy("Category.Read", policy => policy.Requirements.Add(new PermissionRequirement("Category.Read")));
    options.AddPolicy("Category.Update", policy => policy.Requirements.Add(new PermissionRequirement("Category.Update")));
    options.AddPolicy("Category.Delete", policy => policy.Requirements.Add(new PermissionRequirement("Category.Delete")));

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
});

// ─── EF Core DbContext ─────────────────────────────────────────────────────
builder.Services.AddDbContextPool<AppDbContext>(options =>
{
    var connName = builder.Environment.IsDevelopment() ? "dev" : "main";
    options.UseSqlServer(builder.Configuration.GetConnectionString(connName));
});




// ─── AutoMapper, Repos & Services ───────────────────────────────────────────
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// Core repositories
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();

// Catalog repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ISubcategoryRepository, SubcategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();


// Token & email
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Permission management
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

// ─── Controllers & Swagger ─────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Attar Store", Version = "v1" });
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
            new OpenApiSecurityScheme
            {
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

app.MapControllers();
app.ApplyMigrations();
app.Run();






