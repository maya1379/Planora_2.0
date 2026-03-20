using Planora.Domain.Entities;

namespace Planora.Services.Interfaces;

public interface IGroupRepository
{
    Task<IEnumerable<Groups>> GetAllAsync();
    Task<Groups?> GetByIdAsync(int id);
    Task<IEnumerable<Groups>> GetByFacultyAsync(string faculty);
    Task<Groups> AddAsync(Groups groups);
    Task UpdateAsync(Groups groups);
    Task DeleteAsync(int id);
}
