using Planora.Domain.Entities;

namespace Planora.Services.Interfaces;

public interface ISubjectRepository
{
    Task<IEnumerable<Subjects>> GetAllAsync();
    Task<Subjects?> GetByIdAsync(int id);
    Task<Subjects> AddAsync(Subjects subjects);
    Task UpdateAsync(Subjects subjects);
    Task DeleteAsync(int id);
}
