namespace Planora.Services.DTOs;

public class TeacherSearchDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Faculty { get; set; }
    public string? Position { get; set; }
}
