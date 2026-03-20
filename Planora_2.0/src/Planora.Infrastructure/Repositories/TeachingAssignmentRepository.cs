using Microsoft.EntityFrameworkCore;
using Planora.Services.Interfaces;
using Planora.Domain.Entities;
using Planora.Infrastructure.Data;

namespace Planora.Infrastructure.Repositories;

public class TeachingAssignmentRepository : ITeachingAssignmentRepository
{
    private readonly PlanoraDbContext _context;

    public TeachingAssignmentRepository(PlanoraDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TeachingAssignment>> GetAllAsync()
    {
        return await _context.TeachingAssignments
            .Include(ta => ta.Teacher)
            .Include(ta => ta.Subjects)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<TeachingAssignment?> GetByIdAsync(int id)
    {
        return await _context.TeachingAssignments
            .Include(ta => ta.Teacher)
            .Include(ta => ta.Subjects)
            .FirstOrDefaultAsync(ta => ta.Id == id);
    }

    public async Task<IEnumerable<TeachingAssignment>> GetByTeacherIdAsync(string teacherId)
    {
        return await _context.TeachingAssignments
            .Include(ta => ta.Teacher)
            .Include(ta => ta.Subjects)
            .Where(ta => ta.TeacherId == teacherId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<TeachingAssignment>> GetBySubjectIdAsync(int subjectId)
    {
        return await _context.TeachingAssignments
            .Include(ta => ta.Teacher)
            .Include(ta => ta.Subjects)
            .Where(ta => ta.SubjectId == subjectId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<TeachingAssignment> AddAsync(TeachingAssignment assignment)
    {
        _context.TeachingAssignments.Add(assignment);
        await _context.SaveChangesAsync();
        return assignment;
    }

    public async Task UpdateAsync(TeachingAssignment assignment)
    {
        _context.TeachingAssignments.Update(assignment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var assignment = await _context.TeachingAssignments.FindAsync(id);
        if (assignment != null)
        {
            _context.TeachingAssignments.Remove(assignment);
            await _context.SaveChangesAsync();
        }
    }
}
