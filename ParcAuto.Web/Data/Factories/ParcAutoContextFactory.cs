using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ParcAuto.Web.Data;
using System.IO;

namespace ParcAuto.Web.Data.Factories
{
    public class ParcAutoContextFactory : IDesignTimeDbContextFactory<ParcAutoContext>
    {
        public ParcAutoContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ParcAutoContext>();

            optionsBuilder.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));

            return new ParcAutoContext(optionsBuilder.Options);
        }
    }
}
