using Microsoft.EntityFrameworkCore;
using Planora.Services.Interfaces;
using Planora.Domain.Entities;
using Planora.Infrastructure.Data;

namespace Planora.Infrastructure.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly PlanoraDbContext _context;

    public GroupRepository(PlanoraDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Groups>> GetAllAsync()
    {
        return await _context.Groups
            .Include(g => g.GroupDisciplineLists)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Groups?> GetByIdAsync(int id)
    {
        return await _context.Groups
            .Include(g => g.GroupDisciplineLists)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<IEnumerable<Groups>> GetByFacultyAsync(string faculty)
    {
        return await _context.Groups
            .Where(g => g.Faculty == faculty)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Groups> AddAsync(Groups groups)
    {
        _context.Groups.Add(groups);
        await _context.SaveChangesAsync();
        return groups;
    }

    public async Task UpdateAsync(Groups groups)
    {
        _context.Groups.Update(groups);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var groups = await _context.Groups.FindAsync(id);
        if (groups != null)
        {
            _context.Groups.Remove(groups);
            await _context.SaveChangesAsync();
        }
    }
}
