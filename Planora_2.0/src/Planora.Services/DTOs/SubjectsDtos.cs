using Planora.Domain.Enums;

namespace Planora.Services.DTOs;

public class SubjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public LessonType Type { get; set; }
    public string? Requirements { get; set; }
}

public class CreateSubjectDto
{
    public string Name { get; set; } = string.Empty;
    public LessonType Type { get; set; }
    public string? Requirements { get; set; }
}

public class UpdateSubjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public LessonType Type { get; set; }
    public string? Requirements { get; set; }
}
