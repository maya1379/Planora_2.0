using Planora.Domain.Entities;

namespace Planora.Services.Interfaces;

public interface ITeachingAssignmentRepository
{
    Task<IEnumerable<TeachingAssignment>> GetAllAsync();
    Task<TeachingAssignment?> GetByIdAsync(int id);
    Task<IEnumerable<TeachingAssignment>> GetByTeacherIdAsync(string teacherId);
    Task<IEnumerable<TeachingAssignment>> GetBySubjectIdAsync(int subjectId);
    Task<TeachingAssignment> AddAsync(TeachingAssignment assignment);
    Task UpdateAsync(TeachingAssignment assignment);
    Task DeleteAsync(int id);
}
