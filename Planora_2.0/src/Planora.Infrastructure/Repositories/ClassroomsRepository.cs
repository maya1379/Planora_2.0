using Microsoft.EntityFrameworkCore;
using Planora.Services.Interfaces;
using Planora.Domain.Entities;
using Planora.Domain.Enums;
using Planora.Infrastructure.Data;

namespace Planora.Infrastructure.Repositories;

public class ClassroomRepository : IClassroomRepository
{
    private readonly PlanoraDbContext _context;

    public ClassroomRepository(PlanoraDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Classrooms>> GetAllAsync()
    {
        return await _context.Classrooms
            .Include(c => c.Building)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Classrooms?> GetByIdAsync(int id)
    {
        return await _context.Classrooms
            .Include(c => c.Building)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Classrooms>> GetByBuildingIdAsync(int buildingId)
    {
        return await _context.Classrooms
            .Include(c => c.Building)
            .Where(c => c.BuildingId == buildingId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Classrooms>> GetAvailableAsync(DayOfWeekEnum dayOfWeek, int timeSlotId, WeekType weekType)
    {
        var occupiedClassroomIds = await _context.Schedules
            .Where(se => se.DayOfWeek == dayOfWeek
                && se.TimeSlotId == timeSlotId
                && (se.WeekType == weekType || se.WeekType == WeekType.Both || weekType == WeekType.Both))
            .Select(se => se.ClassroomId)
            .ToListAsync();

        return await _context.Classrooms
            .Include(c => c.Building)
            .Where(c => !occupiedClassroomIds.Contains(c.Id))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Classrooms> AddAsync(Classrooms classrooms)
    {
        _context.Classrooms.Add(classrooms);
        await _context.SaveChangesAsync();
        return classrooms;
    }

    public async Task UpdateAsync(Classrooms classrooms)
    {
        _context.Classrooms.Update(classrooms);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var classrooms = await _context.Classrooms.FindAsync(id);
        if (classrooms != null)
        {
            _context.Classrooms.Remove(classrooms);
            await _context.SaveChangesAsync();
        }
    }
}
