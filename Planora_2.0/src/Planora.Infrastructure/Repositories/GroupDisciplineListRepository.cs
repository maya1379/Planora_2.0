using Microsoft.EntityFrameworkCore;
using Planora.Services.Interfaces;
using Planora.Domain.Entities;
using Planora.Infrastructure.Data;

namespace Planora.Infrastructure.Repositories;

public class GroupSubjectRepository : IGroupSubjectRepository
{
    private readonly PlanoraDbContext _context;

    public GroupSubjectRepository(PlanoraDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<GroupDisciplineList>> GetAllAsync()
    {
        return await _context.GroupDisciplineLists
            .Include(gs => gs.Groups)
            .Include(gs => gs.Subjects)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<GroupDisciplineList?> GetByIdAsync(int id)
    {
        return await _context.GroupDisciplineLists
            .Include(gs => gs.Groups)
            .Include(gs => gs.Subjects)
            .FirstOrDefaultAsync(gs => gs.Id == id);
    }

    public async Task<IEnumerable<GroupDisciplineList>> GetByGroupIdAsync(int groupId)
    {
        return await _context.GroupDisciplineLists
            .Include(gs => gs.Groups)
            .Include(gs => gs.Subjects)
            .Where(gs => gs.GroupId == groupId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<GroupDisciplineList>> GetBySubjectIdAsync(int subjectId)
    {
        return await _context.GroupDisciplineLists
            .Include(gs => gs.Groups)
            .Include(gs => gs.Subjects)
            .Where(gs => gs.SubjectId == subjectId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<GroupDisciplineList> AddAsync(GroupDisciplineList groupDisciplineList)
    {
        _context.GroupDisciplineLists.Add(groupDisciplineList);
        await _context.SaveChangesAsync();
        return groupDisciplineList;
    }

    public async Task UpdateAsync(GroupDisciplineList groupDisciplineList)
    {
        _context.GroupDisciplineLists.Update(groupDisciplineList);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var groupDisciplineList = await _context.GroupDisciplineLists.FindAsync(id);
        if (groupDisciplineList != null)
        {
            _context.GroupDisciplineLists.Remove(groupDisciplineList);
            await _context.SaveChangesAsync();
        }
    }
}
