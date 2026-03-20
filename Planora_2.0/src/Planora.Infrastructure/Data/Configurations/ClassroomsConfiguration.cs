using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planora.Domain.Entities;

namespace Planora.Infrastructure.Data.Configurations;

public class ClassroomConfiguration : IEntityTypeConfiguration<Classrooms>
{
    public void Configure(EntityTypeBuilder<Classrooms> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Number)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Capacity)
            .IsRequired();

        builder.HasOne(c => c.Building)
            .WithMany(b => b.Classrooms)
            .HasForeignKey(c => c.BuildingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Schedules)
            .WithOne(se => se.Classrooms)
            .HasForeignKey(se => se.ClassroomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
