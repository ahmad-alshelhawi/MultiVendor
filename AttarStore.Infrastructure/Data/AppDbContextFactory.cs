using AttarStore.Services.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace AttarStore.Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var basePath = Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)                              // <-- now available
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connName = env.Equals("Development", StringComparison.OrdinalIgnoreCase)
                ? "dev"
                : "main";

            var connString = config.GetConnectionString(connName)
                ?? throw new InvalidOperationException($"Connection string '{connName}' not found.");

            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder
                .UseSqlServer(connString)
                .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

            return new AppDbContext(builder.Options);
        }
    }
}
