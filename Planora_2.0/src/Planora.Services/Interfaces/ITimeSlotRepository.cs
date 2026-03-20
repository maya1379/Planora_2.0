using Planora.Domain.Entities;

namespace Planora.Services.Interfaces;

public interface ITimeSlotRepository
{
    Task<IEnumerable<TimeSlot>> GetAllAsync();
    Task<TimeSlot?> GetByIdAsync(int id);
    Task<TimeSlot> AddAsync(TimeSlot timeSlot);
    Task UpdateAsync(TimeSlot timeSlot);
    Task DeleteAsync(int id);
}
