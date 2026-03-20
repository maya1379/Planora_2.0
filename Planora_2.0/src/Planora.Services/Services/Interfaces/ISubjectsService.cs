using Planora.Services.DTOs;

namespace Planora.Services.Services.Interfaces;

public interface ISubjectService
{
    Task<IEnumerable<SubjectDto>> GetAllAsync();
    Task<SubjectDto?> GetByIdAsync(int id);
    Task<SubjectDto> CreateAsync(CreateSubjectDto dto);
    Task UpdateAsync(UpdateSubjectDto dto);
    Task DeleteAsync(int id);
}
