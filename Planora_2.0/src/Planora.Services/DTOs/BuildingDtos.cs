namespace Planora.Services.DTOs;

public class BuildingDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public int ClassroomCount { get; set; }
}

public class CreateBuildingDto
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
}

public class UpdateBuildingDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
}
