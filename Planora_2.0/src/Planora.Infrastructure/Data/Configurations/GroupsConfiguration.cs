using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planora.Domain.Entities;

namespace Planora.Infrastructure.Data.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Groups>
{
    public void Configure(EntityTypeBuilder<Groups> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(g => g.Faculty)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(g => g.StudentCount)
            .IsRequired();

        builder.HasIndex(g => g.Name).IsUnique();

        builder.HasMany(g => g.GroupDisciplineLists)
            .WithOne(gs => gs.Groups)
            .HasForeignKey(gs => gs.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(g => g.Schedules)
            .WithOne(se => se.Groups)
            .HasForeignKey(se => se.GroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
