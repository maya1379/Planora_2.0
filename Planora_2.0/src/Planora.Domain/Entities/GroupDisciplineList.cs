using Planora.Domain.Enums;

namespace Planora.Domain.Entities;

public class GroupDisciplineList
{
    public int Id { get; set; }

    public int HoursPerWeek { get; set; }

    public LessonType LessonType { get; set; }

    public int GroupId { get; set; }

    public int SubjectId { get; set; }

    public virtual Groups Groups { get; set; } = null!;

    public virtual Subjects Subjects { get; set; } = null!;
}
