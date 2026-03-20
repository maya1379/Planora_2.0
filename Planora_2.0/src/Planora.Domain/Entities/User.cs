using Microsoft.AspNetCore.Identity;
using Planora.Domain.Enums;

namespace Planora.Domain.Entities;

public class User : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    public string? Faculty { get; set; }

    public string? Position { get; set; }

    public UserRole Role { get; set; }

    public int? GroupId { get; set; }

    public virtual Groups? Groups { get; set; }

    public virtual ICollection<TeachingAssignment> TeachingAssignments { get; set; } = new List<TeachingAssignment>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
