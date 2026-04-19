namespace Planora.Services.DTOs;

public class BuildingStatisticsDto
{
    public int BuildingId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ClassroomsCount { get; set; }
    public int TotalSchedulesCount { get; set; }
}