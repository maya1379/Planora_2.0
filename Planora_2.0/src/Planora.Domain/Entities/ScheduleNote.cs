using System;

namespace Planora.Domain.Entities;

public class ScheduleNote
{
    public int Id { get; set; }
    public int ScheduleId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Schedule Schedule { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
