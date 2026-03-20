using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planora.Domain.Entities;

namespace Planora.Infrastructure.Data.Configurations;

public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.HasKey(ts => ts.Id);

        builder.Property(ts => ts.Number)
            .IsRequired();

        builder.Property(ts => ts.StartTime)
            .IsRequired();

        builder.Property(ts => ts.EndTime)
            .IsRequired();

        builder.HasIndex(ts => ts.Number).IsUnique();
    }
}
