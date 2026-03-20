using Planora.Services.DTOs;

namespace Planora.Services.Services.Interfaces;

public interface ITeachingAssignmentService
{
    Task<IEnumerable<TeachingAssignmentDto>> GetAllAsync();
    Task<TeachingAssignmentDto?> GetByIdAsync(int id);
    Task<IEnumerable<TeachingAssignmentDto>> GetByTeacherIdAsync(string teacherId);
    Task<TeachingAssignmentDto> CreateAsync(CreateTeachingAssignmentDto dto);
    Task UpdateAsync(UpdateTeachingAssignmentDto dto);
    Task DeleteAsync(int id);
}
