using Planora.Domain.Entities;
using Planora.Domain.Enums;

namespace Planora.Services.Interfaces;

public interface IScheduleEntryRepository
{
    Task<IEnumerable<Schedule>> GetAllAsync();
    Task<Schedule?> GetByIdAsync(int id);
    Task<IEnumerable<Schedule>> GetByGroupIdAsync(int groupId);
    Task<IEnumerable<Schedule>> GetByTeacherIdAsync(string teacherId);
    Task<IEnumerable<Schedule>> GetByClassroomIdAsync(int classroomId);
    Task<IEnumerable<Schedule>> GetByDayAndTimeSlotAsync(DayOfWeekEnum dayOfWeek, int timeSlotId);
    Task<Schedule> AddAsync(Schedule entry);
    Task AddRangeAsync(IEnumerable<Schedule> entries);
    Task UpdateAsync(Schedule entry);
    Task DeleteAsync(int id);
    Task DeleteAllAsync();
}
