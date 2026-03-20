using Planora.Domain.Entities;
using Planora.Domain.Enums;

namespace Planora.Services.Interfaces;

public interface IClassroomRepository
{
    Task<IEnumerable<Classrooms>> GetAllAsync();
    Task<Classrooms?> GetByIdAsync(int id);
    Task<IEnumerable<Classrooms>> GetByBuildingIdAsync(int buildingId);
    Task<IEnumerable<Classrooms>> GetAvailableAsync(DayOfWeekEnum dayOfWeek, int timeSlotId, WeekType weekType);
    Task<Classrooms> AddAsync(Classrooms classrooms);
    Task UpdateAsync(Classrooms classrooms);
    Task DeleteAsync(int id);
}
