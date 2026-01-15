using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ParcAuto.Web.Models;



namespace ParcAuto.Web.Data
{
    public class ParcAutoContext : IdentityDbContext<ApplicationUser>
    {
        public ParcAutoContext(DbContextOptions<ParcAutoContext> options)
            : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<Driver> Drivers => Set<Driver>();
        public DbSet<Reservation> Reservations => Set<Reservation>();

        public DbSet<Maintenance> Maintenances => Set<Maintenance>();

        public DbSet<FuelLog> FuelLogs => Set<FuelLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FuelLog>()
                .Property(f => f.Liters).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<FuelLog>()
                .Property(f => f.Price).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Maintenance>()
                .Property(m => m.Cost).HasColumnType("decimal(18,2)");
        }
    }
}
