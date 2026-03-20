using Planora.Domain.Enums;

namespace Planora.Domain.Entities;

public class Subjects
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public LessonType Type { get; set; }

    public string? Requirements { get; set; } 

    public virtual ICollection<TeachingAssignment> TeachingAssignments { get; set; } = new List<TeachingAssignment>();

    public virtual ICollection<GroupDisciplineList> GroupDisciplineLists { get; set; } = new List<GroupDisciplineList>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
