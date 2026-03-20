using Microsoft.EntityFrameworkCore;
using Planora.Services.Interfaces;
using Planora.Domain.Entities;
using Planora.Infrastructure.Data;

namespace Planora.Infrastructure.Repositories;

public class TimeSlotRepository : ITimeSlotRepository
{
    private readonly PlanoraDbContext _context;

    public TimeSlotRepository(PlanoraDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TimeSlot>> GetAllAsync()
    {
        return await _context.TimeSlots
            .OrderBy(ts => ts.Number)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<TimeSlot?> GetByIdAsync(int id)
    {
        return await _context.TimeSlots.FindAsync(id);
    }

    public async Task<TimeSlot> AddAsync(TimeSlot timeSlot)
    {
        _context.TimeSlots.Add(timeSlot);
        await _context.SaveChangesAsync();
        return timeSlot;
    }

    public async Task UpdateAsync(TimeSlot timeSlot)
    {
        _context.TimeSlots.Update(timeSlot);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var timeSlot = await _context.TimeSlots.FindAsync(id);
        if (timeSlot != null)
        {
            _context.TimeSlots.Remove(timeSlot);
            await _context.SaveChangesAsync();
        }
    }
}
