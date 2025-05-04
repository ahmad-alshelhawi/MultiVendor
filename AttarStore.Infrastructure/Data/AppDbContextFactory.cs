using AttarStore.Services.Data;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AttarStore.Infrastructure.Data
{
    /// <summary>
    /// Used by EF Core tools at design time to create the DbContext,
    /// so we can configure warnings (e.g. PendingModelChanges) here.
    /// </summary>
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();

            // TODO: replace with your real connection string or read from env
            var conn = "Server=localhost,1433;Database=AttarStore_Db;User Id=sa;Password=11;Encrypt=false;";

            builder
                .UseSqlServer(conn)
                // suppress the warning about model snapshot changing
                .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

            return new AppDbContext(builder.Options);
        }
    }
}
