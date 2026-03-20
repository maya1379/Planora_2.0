namespace Planora.Services.DTOs;

public class TeacherLocationDto
{
    public string TeacherId { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public bool IsTeachingNow { get; set; }
    public string? ClassroomNumber { get; set; }
    public string? BuildingName { get; set; }
    public string? SubjectName { get; set; }
    public int? TimeSlotNumber { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string? GroupName { get; set; }
}
