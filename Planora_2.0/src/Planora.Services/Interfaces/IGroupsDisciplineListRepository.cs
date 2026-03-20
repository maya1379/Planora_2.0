using Planora.Domain.Entities;

namespace Planora.Services.Interfaces;

public interface IGroupSubjectRepository
{
    Task<IEnumerable<GroupDisciplineList>> GetAllAsync();
    Task<GroupDisciplineList?> GetByIdAsync(int id);
    Task<IEnumerable<GroupDisciplineList>> GetByGroupIdAsync(int groupId);
    Task<IEnumerable<GroupDisciplineList>> GetBySubjectIdAsync(int subjectId);
    Task<GroupDisciplineList> AddAsync(GroupDisciplineList groupDisciplineList);
    Task UpdateAsync(GroupDisciplineList groupDisciplineList);
    Task DeleteAsync(int id);
}
