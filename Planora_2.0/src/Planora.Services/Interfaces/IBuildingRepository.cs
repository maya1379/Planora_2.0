using Planora.Domain.Entities;

namespace Planora.Services.Interfaces;

public interface IBuildingRepository
{
    Task<IEnumerable<Building>> GetAllAsync();
    Task<Building?> GetByIdAsync(int id);
    Task<Building> AddAsync(Building building);
    Task UpdateAsync(Building building);
    Task DeleteAsync(int id);
}
