using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Portal.Data.Entities;
using System.IO;

namespace Portal.Data
{
    public class PortalDbContextFactory : IDesignTimeDbContextFactory<PortalDbContext>
    {
        public PortalDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Portal.Web"))
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<PortalDbContext>();
            var connectionString = configuration.GetConnectionString("DevConnection");
            builder.UseSqlServer(connectionString);

            return new PortalDbContext(builder.Options);
        }
    }
}
