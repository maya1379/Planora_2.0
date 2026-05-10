using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planora.Domain.Entities;

namespace Planora.Infrastructure.Data.Configurations;

public class TeachingAssignmentConfiguration : IEntityTypeConfiguration<TeachingAssignment>
{
    public void Configure(EntityTypeBuilder<TeachingAssignment> builder)
    {
        builder.HasKey(ta => ta.Id);

        builder.Property(ta => ta.HoursPerWeek)
            .IsRequired();

        builder.HasOne(ta => ta.Teacher)
            .WithMany(t => t.TeachingAssignments)
            .HasForeignKey(ta => ta.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ta => ta.Subjects)
            .WithMany(s => s.TeachingAssignments)
            .HasForeignKey(ta => ta.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ta => new { ta.TeacherId, ta.SubjectId, ta.GroupId }).IsUnique();
    }
}
