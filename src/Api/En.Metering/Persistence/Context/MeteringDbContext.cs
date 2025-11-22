using En.Metering.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace En.Metering.Persistence.Context
{
    public class MeteringDbContext : DbContext
    {
        public MeteringDbContext(DbContextOptions<MeteringDbContext> options)
            : base(options) { }

        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<MeterReading> MeterReadings => Set<MeterReading>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MeteringDbContext).Assembly);
            modelBuilder.Entity<Account>().HasData(DataSeed.GetSeed());
        }
    }
}
