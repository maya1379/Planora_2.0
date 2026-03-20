using Planora.Services.DTOs;

namespace Planora.Services.Services.Interfaces;

public interface IGroupSubjectService
{
    Task<IEnumerable<GroupSubjectDto>> GetAllAsync();
    Task<GroupSubjectDto?> GetByIdAsync(int id);
    Task<IEnumerable<GroupSubjectDto>> GetByGroupIdAsync(int groupId);
    Task<GroupSubjectDto> CreateAsync(CreateGroupSubjectDto dto);
    Task UpdateAsync(UpdateGroupSubjectDto dto);
    Task DeleteAsync(int id);
}
