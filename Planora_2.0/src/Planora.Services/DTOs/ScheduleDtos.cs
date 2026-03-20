using Planora.Domain.Enums;

namespace Planora.Services.DTOs;

public class ScheduleEntryDto
{
    public int Id { get; set; }
    public DayOfWeekEnum DayOfWeek { get; set; }
    public WeekType WeekType { get; set; }
    public int TimeSlotId { get; set; }
    public int TimeSlotNumber { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int ClassroomId { get; set; }
    public string ClassroomNumber { get; set; } = string.Empty;
    public string BuildingName { get; set; } = string.Empty;
    public string TeacherId { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public LessonType LessonType { get; set; }
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
}

public class CreateScheduleEntryDto
{
    public DayOfWeekEnum DayOfWeek { get; set; }
    public WeekType WeekType { get; set; }
    public int TimeSlotId { get; set; }
    public int ClassroomId { get; set; }
    public string TeacherId { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public int GroupId { get; set; }
}

public class ScheduleGenerationResultDto
{
    public bool Success { get; set; }
    public int TotalEntriesCreated { get; set; }
    public List<string> Warnings { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public List<ScheduleEntryDto> Entries { get; set; } = new();
}
