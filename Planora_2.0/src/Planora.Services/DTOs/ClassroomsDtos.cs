namespace Planora.Services.DTOs;

public class ClassroomDto
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public bool HasComputers { get; set; }
    public bool HasProjector { get; set; }
    public string Faculty { get; set; } = string.Empty;
    public int BuildingId { get; set; }
    public string BuildingName { get; set; } = string.Empty;
}

public class CreateClassroomDto
{
    public string Number { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public bool HasComputers { get; set; }
    public bool HasProjector { get; set; }
    public string Faculty { get; set; } = string.Empty;
    public int BuildingId { get; set; }
}

public class UpdateClassroomDto
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public bool HasComputers { get; set; }
    public bool HasProjector { get; set; }
    public string Faculty { get; set; } = string.Empty;
    public int BuildingId { get; set; }
}
