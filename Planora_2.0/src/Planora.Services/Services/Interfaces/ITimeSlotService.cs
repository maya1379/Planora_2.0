using Planora.Services.DTOs;

namespace Planora.Services.Services.Interfaces;

public interface ITimeSlotService
{
    Task<IEnumerable<TimeSlotDto>> GetAllAsync();
    Task<TimeSlotDto?> GetByIdAsync(int id);
    Task<TimeSlotDto> CreateAsync(CreateTimeSlotDto dto);
    Task UpdateAsync(TimeSlotDto dto);
    Task DeleteAsync(int id);
}
