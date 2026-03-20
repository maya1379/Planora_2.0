using Planora.Services.DTOs;
using Planora.Domain.Enums;

namespace Planora.Services.Services.Interfaces;

public interface IScheduleService
{
    Task<IEnumerable<ScheduleEntryDto>> GetAllAsync();
    Task<ScheduleEntryDto?> GetByIdAsync(int id);
    Task<IEnumerable<ScheduleEntryDto>> GetByGroupIdAsync(int groupId);
    Task<IEnumerable<ScheduleEntryDto>> GetByTeacherIdAsync(string teacherId);
    Task<IEnumerable<ScheduleEntryDto>> GetByClassroomIdAsync(int classroomId);
    Task<IEnumerable<ScheduleEntryDto>> GetTodayByGroupIdAsync(int groupId);
    Task<IEnumerable<ScheduleEntryDto>> GetTodayByTeacherIdAsync(string teacherId);
    Task<TeacherLocationDto?> FindTeacherLocationAsync(string teacherId);
    Task<IEnumerable<TeacherSearchDto>> SearchTeachersAsync(string query);
    Task<ScheduleEntryDto> CreateAsync(CreateScheduleEntryDto dto);
    Task UpdateAsync(int id, CreateScheduleEntryDto dto);
    Task DeleteAsync(int id);
    Task DeleteAllAsync();
}
