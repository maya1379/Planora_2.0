using Planora.Domain.Enums;

namespace Planora.Services.DTOs;

public class GroupSubjectDto
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int HoursPerWeek { get; set; }
    public LessonType LessonType { get; set; }
}

public class CreateGroupSubjectDto
{
    public int GroupId { get; set; }
    public int SubjectId { get; set; }
    public int HoursPerWeek { get; set; }
    public LessonType LessonType { get; set; }
}

public class UpdateGroupSubjectDto
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public int SubjectId { get; set; }
    public int HoursPerWeek { get; set; }
    public LessonType LessonType { get; set; }
}
