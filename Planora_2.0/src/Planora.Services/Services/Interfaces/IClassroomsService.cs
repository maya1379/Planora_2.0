using Planora.Services.DTOs;
using Planora.Domain.Enums;

namespace Planora.Services.Services.Interfaces;

public interface IClassroomService
{
    Task<IEnumerable<ClassroomDto>> GetAllAsync();
    Task<ClassroomDto?> GetByIdAsync(int id);
    Task<IEnumerable<ClassroomDto>> GetByBuildingIdAsync(int buildingId);
    Task<IEnumerable<ClassroomDto>> GetAvailableAsync(DayOfWeekEnum dayOfWeek, int timeSlotId, WeekType weekType);
    Task<IEnumerable<ClassroomDto>> FindFreeClassroomsNowAsync();
    Task<ClassroomDto> CreateAsync(CreateClassroomDto dto);
    Task UpdateAsync(UpdateClassroomDto dto);
    Task DeleteAsync(int id);
}
