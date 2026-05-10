using Microsoft.EntityFrameworkCore;
using Planora.Domain.Entities;
using Planora.Infrastructure.Data;
using Planora.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planora.Infrastructure.Repositories;

public class ScheduleNoteRepository : IScheduleNoteRepository
{
    private readonly PlanoraDbContext _context;

    public ScheduleNoteRepository(PlanoraDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ScheduleNote>> GetNotesForScheduleAsync(int scheduleId, string userId)
    {
        return await _context.ScheduleNotes
            .Include(n => n.User)
            .Where(n => n.ScheduleId == scheduleId && n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<ScheduleNote> AddAsync(ScheduleNote note)
    {
        _context.ScheduleNotes.Add(note);
        await _context.SaveChangesAsync();
        
        return await _context.ScheduleNotes
            .Include(n => n.User)
            .FirstAsync(n => n.Id == note.Id);
    }

    public async Task<ScheduleNote?> GetByIdAsync(int id)
    {
        return await _context.ScheduleNotes.FindAsync(id);
    }

    public async Task DeleteAsync(ScheduleNote note)
    {
        _context.ScheduleNotes.Remove(note);
        await _context.SaveChangesAsync();
    }
}
