using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Planora.Domain.Entities;

namespace Planora.Infrastructure.Data.Configurations;

public class GroupSubjectConfiguration : IEntityTypeConfiguration<GroupDisciplineList>
{
    public void Configure(EntityTypeBuilder<GroupDisciplineList> builder)
    {
        builder.HasKey(gs => gs.Id);

        builder.Property(gs => gs.HoursPerWeek)
            .IsRequired();

        builder.Property(gs => gs.LessonType)
            .IsRequired()
            .HasConversion<string>();

        builder.HasOne(gs => gs.Groups)
            .WithMany(g => g.GroupDisciplineLists)
            .HasForeignKey(gs => gs.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(gs => gs.Subjects)
            .WithMany(s => s.GroupDisciplineLists)
            .HasForeignKey(gs => gs.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
