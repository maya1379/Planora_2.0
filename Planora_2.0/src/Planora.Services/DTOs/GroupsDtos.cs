using Planora.Domain.Enums;

namespace Planora.Services.DTOs;

public class GroupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public int StudentCount { get; set; }
}

public class CreateGroupDto
{
    public string Name { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public int StudentCount { get; set; }
}

public class UpdateGroupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;
    public int StudentCount { get; set; }
}
