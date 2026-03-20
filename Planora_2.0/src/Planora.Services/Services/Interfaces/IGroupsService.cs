using Planora.Services.DTOs;

namespace Planora.Services.Services.Interfaces;

public interface IGroupService
{
    Task<IEnumerable<GroupDto>> GetAllAsync();
    Task<GroupDto?> GetByIdAsync(int id);
    Task<IEnumerable<GroupDto>> GetByFacultyAsync(string faculty);
    Task<GroupDto> CreateAsync(CreateGroupDto dto);
    Task UpdateAsync(UpdateGroupDto dto);
    Task DeleteAsync(int id);
}
