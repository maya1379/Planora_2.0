namespace Planora.Domain.Entities;

public class TimeSlot
{
    public int Id { get; set; }

    public int Number { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
