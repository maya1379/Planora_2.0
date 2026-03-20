using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planora.Domain.Entities;

namespace Planora.Infrastructure.Data.Configurations;

public class ScheduleEntryConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.HasKey(se => se.Id);

        builder.Property(se => se.DayOfWeek)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(se => se.WeekType)
            .IsRequired()
            .HasConversion<string>();

        builder.HasOne(se => se.TimeSlot)
            .WithMany(ts => ts.Schedules)
            .HasForeignKey(se => se.TimeSlotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(se => se.Classrooms)
            .WithMany(c => c.Schedules)
            .HasForeignKey(se => se.ClassroomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(se => se.Teacher)
            .WithMany(t => t.Schedules)
            .HasForeignKey(se => se.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(se => se.Subjects)
            .WithMany(s => s.Schedules)
            .HasForeignKey(se => se.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(se => se.Groups)
            .WithMany(g => g.Schedules)
            .HasForeignKey(se => se.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(se => new { se.DayOfWeek, se.TimeSlotId, se.WeekType });
    }
}
