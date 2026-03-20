using Microsoft.EntityFrameworkCore;
using Planora.Services.Interfaces;
using Planora.Domain.Entities;
using Planora.Infrastructure.Data;

namespace Planora.Infrastructure.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly PlanoraDbContext _context;

    public SubjectRepository(PlanoraDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Subjects>> GetAllAsync()
    {
        return await _context.Subjects.AsNoTracking().ToListAsync();
    }

    public async Task<Subjects?> GetByIdAsync(int id)
    {
        return await _context.Subjects.FindAsync(id);
    }

    public async Task<Subjects> AddAsync(Subjects subjects)
    {
        _context.Subjects.Add(subjects);
        await _context.SaveChangesAsync();
        return subjects;
    }

    public async Task UpdateAsync(Subjects subjects)
    {
        _context.Subjects.Update(subjects);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var subjects = await _context.Subjects.FindAsync(id);
        if (subjects != null)
        {
            _context.Subjects.Remove(subjects);
            await _context.SaveChangesAsync();
        }
    }
}
