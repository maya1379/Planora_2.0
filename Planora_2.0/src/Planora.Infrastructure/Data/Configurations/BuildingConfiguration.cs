using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planora.Domain.Entities;

namespace Planora.Infrastructure.Data.Configurations;

public class BuildingConfiguration : IEntityTypeConfiguration<Building>
{
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Address)
            .HasMaxLength(300);

        builder.HasMany(b => b.Classrooms)
            .WithOne(c => c.Building)
            .HasForeignKey(c => c.BuildingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
