namespace Planora.Services.DTOs;

public class TimeSlotDto
{
    public int Id { get; set; }
    public int Number { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}

public class CreateTimeSlotDto
{
    public int Number { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
