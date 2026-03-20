using Planora.Domain.Enums;

namespace Planora.Domain.Entities;

public class Schedule
{
    public int Id { get; set; }

    public DayOfWeekEnum DayOfWeek { get; set; }

    public WeekType WeekType { get; set; }

    public int TimeSlotId { get; set; }

    public int ClassroomId { get; set; }

    public string TeacherId { get; set; } = string.Empty;

    public int SubjectId { get; set; }

    public int GroupId { get; set; }

    public virtual TimeSlot TimeSlot { get; set; } = null!;

    public virtual Classrooms Classrooms { get; set; } = null!;

    public virtual User Teacher { get; set; } = null!;

    public virtual Subjects Subjects { get; set; } = null!;

    public virtual Groups Groups { get; set; } = null!;
}
