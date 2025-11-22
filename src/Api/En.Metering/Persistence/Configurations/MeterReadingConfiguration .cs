using En.Metering.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace En.Metering.Persistence.Configurations
{
    public class MeterReadingConfiguration : IEntityTypeConfiguration<MeterReading>
    {
        public void Configure(EntityTypeBuilder<MeterReading> builder)
        {
            builder.HasKey(x => x.Id);

            //En dev; you could also add the meter reading value if you want to be very strict about duplicate readings.
            builder.HasIndex(x => new { x.AccountId, x.MeterReadingDateTime })
                .IsUnique();

            builder.Property(x => x.MeterReadingValue).IsRequired();

            builder.HasOne(m => m.Account)
                .WithMany(a => a.MeterReadings)
                .HasForeignKey(m => m.AccountId);
        }
    }
}
