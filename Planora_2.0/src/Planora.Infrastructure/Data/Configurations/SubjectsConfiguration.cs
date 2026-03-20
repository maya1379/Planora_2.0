using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planora.Domain.Entities;

namespace Planora.Infrastructure.Data.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subjects>
{
    public void Configure(EntityTypeBuilder<Subjects> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.HasMany(s => s.TeachingAssignments)
            .WithOne(ta => ta.Subjects)
            .HasForeignKey(ta => ta.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.GroupDisciplineLists)
            .WithOne(gs => gs.Subjects)
            .HasForeignKey(gs => gs.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Schedules)
            .WithOne(se => se.Subjects)
            .HasForeignKey(se => se.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
