using System;

namespace Planora.Services.DTOs;

public class ScheduleNoteDto
{
    public int Id { get; set; }
    public int ScheduleId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateScheduleNoteDto
{
    public int ScheduleId { get; set; }
    public string Content { get; set; } = string.Empty;
}
