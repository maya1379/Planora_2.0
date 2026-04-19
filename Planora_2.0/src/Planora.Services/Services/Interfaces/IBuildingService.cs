using Planora.Services.DTOs;

namespace Planora.Services.Services.Interfaces;

public interface IBuildingService
{
    Task<IEnumerable<BuildingDto>> GetAllAsync();
    Task<BuildingDto?> GetByIdAsync(int id);
    Task<BuildingDto> CreateAsync(CreateBuildingDto dto);
    Task UpdateAsync(UpdateBuildingDto dto);
    Task DeleteAsync(int id);
    Task<BuildingStatisticsDto?> GetStatisticsAsync(int id);
}
