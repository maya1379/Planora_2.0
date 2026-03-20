namespace Planora.Domain.Entities;

public class TeachingAssignment
{
    public int Id { get; set; }

    public int HoursPerWeek { get; set; }

    public string TeacherId { get; set; } = string.Empty;

    public int SubjectId { get; set; }

    public virtual User Teacher { get; set; } = null!;

    public virtual Subjects Subjects { get; set; } = null!;
}
