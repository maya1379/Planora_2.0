namespace Planora.Services.DTOs;

public class TeachingAssignmentDto
{
    public int Id { get; set; }
    public string TeacherId { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public int HoursPerWeek { get; set; }
}

public class CreateTeachingAssignmentDto
{
    public string TeacherId { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public int GroupId { get; set; }
    public int HoursPerWeek { get; set; }
}

public class UpdateTeachingAssignmentDto
{
    public int Id { get; set; }
    public string TeacherId { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public int GroupId { get; set; }
    public int HoursPerWeek { get; set; }
}
