using Microsoft.EntityFrameworkCore;
using Planora.Services.Interfaces;
using Planora.Domain.Entities;
using Planora.Domain.Enums;
using Planora.Infrastructure.Data;

namespace Planora.Infrastructure.Repositories;

public class ScheduleEntryRepository : IScheduleEntryRepository
{
    private readonly PlanoraDbContext _context;

    public ScheduleEntryRepository(PlanoraDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Schedule>> GetAllAsync()
    {
        return await _context.Schedules
            .Include(se => se.TimeSlot)
            .Include(se => se.Classrooms).ThenInclude(c => c.Building)
            .Include(se => se.Teacher)
            .Include(se => se.Subjects)
            .Include(se => se.Groups)
            .OrderBy(se => se.DayOfWeek)
            .ThenBy(se => se.TimeSlot.Number)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Schedule?> GetByIdAsync(int id)
    {
        return await _context.Schedules
            .Include(se => se.TimeSlot)
            .Include(se => se.Classrooms).ThenInclude(c => c.Building)
            .Include(se => se.Teacher)
            .Include(se => se.Subjects)
            .Include(se => se.Groups)
            .FirstOrDefaultAsync(se => se.Id == id);
    }

    public async Task<IEnumerable<Schedule>> GetByGroupIdAsync(int groupId)
    {
        return await _context.Schedules
            .Include(se => se.TimeSlot)
            .Include(se => se.Classrooms).ThenInclude(c => c.Building)
            .Include(se => se.Teacher)
            .Include(se => se.Subjects)
            .Include(se => se.Groups)
            .Where(se => se.GroupId == groupId)
            .OrderBy(se => se.DayOfWeek)
            .ThenBy(se => se.TimeSlot.Number)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Schedule>> GetByTeacherIdAsync(string teacherId)
    {
        return await _context.Schedules
            .Include(se => se.TimeSlot)
            .Include(se => se.Classrooms).ThenInclude(c => c.Building)
            .Include(se => se.Teacher)
            .Include(se => se.Subjects)
            .Include(se => se.Groups)
            .Where(se => se.TeacherId == teacherId)
            .OrderBy(se => se.DayOfWeek)
            .ThenBy(se => se.TimeSlot.Number)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Schedule>> GetByClassroomIdAsync(int classroomId)
    {
        return await _context.Schedules
            .Include(se => se.TimeSlot)
            .Include(se => se.Classrooms).ThenInclude(c => c.Building)
            .Include(se => se.Teacher)
            .Include(se => se.Subjects)
            .Include(se => se.Groups)
            .Where(se => se.ClassroomId == classroomId)
            .OrderBy(se => se.DayOfWeek)
            .ThenBy(se => se.TimeSlot.Number)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Schedule>> GetByDayAndTimeSlotAsync(DayOfWeekEnum dayOfWeek, int timeSlotId)
    {
        return await _context.Schedules
            .Include(se => se.TimeSlot)
            .Include(se => se.Classrooms).ThenInclude(c => c.Building)
            .Include(se => se.Teacher)
            .Include(se => se.Subjects)
            .Include(se => se.Groups)
            .Where(se => se.DayOfWeek == dayOfWeek && se.TimeSlotId == timeSlotId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Schedule> AddAsync(Schedule entry)
    {
        _context.Schedules.Add(entry);
        await _context.SaveChangesAsync();
        return entry;
    }

    public async Task AddRangeAsync(IEnumerable<Schedule> entries)
    {
        _context.Schedules.AddRange(entries);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Schedule entry)
    {
        _context.Schedules.Update(entry);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entry = await _context.Schedules.FindAsync(id);
        if (entry != null)
        {
            _context.Schedules.Remove(entry);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAllAsync()
    {
        _context.Schedules.RemoveRange(_context.Schedules);
        await _context.SaveChangesAsync();
    }
}
