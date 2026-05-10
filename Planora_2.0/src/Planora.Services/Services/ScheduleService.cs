using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Planora.Services.DTOs;
using Planora.Services.Interfaces;
using Planora.Services.Services.Interfaces;
using Planora.Domain.Entities;
using Planora.Domain.Entities;
using Planora.Domain.Constants;
using Planora.Domain.Enums;

namespace Planora.Services.Services;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleEntryRepository _repository;
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    public ScheduleService(
        IScheduleEntryRepository repository,
        ITimeSlotRepository timeSlotRepository,
        UserManager<User> userManager,
        IMapper mapper)
    {
        _repository = repository;
        _timeSlotRepository = timeSlotRepository;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ScheduleEntryDto>> GetAllAsync()
    {
        var entries = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ScheduleEntryDto>>(entries);
    }

    public async Task<ScheduleEntryDto?> GetByIdAsync(int id)
    {
        var entry = await _repository.GetByIdAsync(id);
        return entry == null ? null : _mapper.Map<ScheduleEntryDto>(entry);
    }

    public async Task<IEnumerable<ScheduleEntryDto>> GetByGroupIdAsync(int groupId)
    {
        var entries = await _repository.GetByGroupIdAsync(groupId);
        return _mapper.Map<IEnumerable<ScheduleEntryDto>>(entries);
    }

    public async Task<IEnumerable<ScheduleEntryDto>> GetByTeacherIdAsync(string teacherId)
    {
        var entries = await _repository.GetByTeacherIdAsync(teacherId);
        return _mapper.Map<IEnumerable<ScheduleEntryDto>>(entries);
    }

    public async Task<IEnumerable<ScheduleEntryDto>> GetByClassroomIdAsync(int classroomId)
    {
        var entries = await _repository.GetByClassroomIdAsync(classroomId);
        return _mapper.Map<IEnumerable<ScheduleEntryDto>>(entries);
    }

    public async Task<IEnumerable<ScheduleEntryDto>> GetTodayByGroupIdAsync(int groupId)
    {
        var allForGroup = await _repository.GetByGroupIdAsync(groupId);
        return FilterForToday(allForGroup);
    }

    public async Task<IEnumerable<ScheduleEntryDto>> GetTodayByTeacherIdAsync(string teacherId)
    {
        var allForTeacher = await _repository.GetByTeacherIdAsync(teacherId);
        return FilterForToday(allForTeacher);
    }

    public async Task<TeacherLocationDto?> FindTeacherLocationAsync(string teacherId)
    {
        var teacher = _userManager.Users.FirstOrDefault(u => u.Id == teacherId);
        if (teacher == null) return null;

        var now = DateTime.Now;
        var currentDay = ClassroomService.GetCurrentDayOfWeekEnum(now.DayOfWeek);
        if (currentDay == null)
        {
            return new TeacherLocationDto
            {
                TeacherId = teacher.Id,
                TeacherName = teacher.FullName,
                IsTeachingNow = false
            };
        }

        var timeSlots = await _timeSlotRepository.GetAllAsync();
        var currentTime = now.TimeOfDay;
        var currentTimeSlot = timeSlots.FirstOrDefault(ts =>
            ts.StartTime <= currentTime && ts.EndTime >= currentTime);

        if (currentTimeSlot == null)
        {
            return new TeacherLocationDto
            {
                TeacherId = teacher.Id,
                TeacherName = teacher.FullName,
                IsTeachingNow = false
            };
        }

        var entries = await _repository.GetByDayAndTimeSlotAsync(currentDay.Value, currentTimeSlot.Id);
        var weekNumber = System.Globalization.CultureInfo.CurrentCulture.Calendar
            .GetWeekOfYear(now, System.Globalization.CalendarWeekRule.FirstFourDayWeek, System.DayOfWeek.Monday);
        var currentWeekType = weekNumber % 2 == 1 ? WeekType.Numerator : WeekType.Denominator;

        var teacherEntry = entries.FirstOrDefault(e =>
            e.TeacherId == teacherId &&
            (e.WeekType == WeekType.Both || e.WeekType == currentWeekType));

        if (teacherEntry == null)
        {
            return new TeacherLocationDto
            {
                TeacherId = teacher.Id,
                TeacherName = teacher.FullName,
                IsTeachingNow = false
            };
        }

        return new TeacherLocationDto
        {
            TeacherId = teacher.Id,
            TeacherName = teacher.FullName,
            IsTeachingNow = true,
            ClassroomNumber = teacherEntry.Classrooms?.Number,
            BuildingName = teacherEntry.Classrooms?.Building?.Name,
            SubjectName = teacherEntry.Subjects?.Name,
            TimeSlotNumber = teacherEntry.TimeSlot?.Number,
            StartTime = teacherEntry.TimeSlot?.StartTime,
            EndTime = teacherEntry.TimeSlot?.EndTime,
            GroupName = teacherEntry.Groups?.Name
        };
    }

    public async Task<IEnumerable<TeacherSearchDto>> SearchTeachersAsync(string query)
    {
        var normalizedQuery = query?.Trim().ToLower() ?? string.Empty;
        var teachersInRole = await _userManager.GetUsersInRoleAsync(AppRoles.Teacher);
        var teachers = teachersInRole
            .Where(u => string.IsNullOrEmpty(normalizedQuery) ||
                        u.FullName.ToLower().Contains(normalizedQuery))
            .Select(u => new TeacherSearchDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Faculty = u.Faculty,
                Position = u.Position
            });

        return teachers;
    }

    public async Task<ScheduleEntryDto> CreateAsync(CreateScheduleEntryDto dto)
    {
        var entry = _mapper.Map<Schedule>(dto);
        var created = await _repository.AddAsync(entry);
        return _mapper.Map<ScheduleEntryDto>(created);
    }

    public async Task UpdateAsync(int id, CreateScheduleEntryDto dto)
    {
        var existing = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Schedule with Id {id} not found.");

        existing.DayOfWeek = dto.DayOfWeek;
        existing.WeekType = dto.WeekType;
        existing.TimeSlotId = dto.TimeSlotId;
        existing.ClassroomId = dto.ClassroomId;
        existing.TeacherId = dto.TeacherId;
        existing.SubjectId = dto.SubjectId;
        existing.GroupId = dto.GroupId;

        await _repository.UpdateAsync(existing);
    }

    public async Task UpdateOnlineStatusAsync(int id, UpdateScheduleOnlineStatusDto dto)
    {
        var existing = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Schedule with Id {id} not found.");

        existing.IsOnline = dto.IsOnline;
        existing.MeetingLink = dto.MeetingLink;

        await _repository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task DeleteAllAsync()
    {
        await _repository.DeleteAllAsync();
    }

    private IEnumerable<ScheduleEntryDto> FilterForToday(IEnumerable<Schedule> entries)
    {
        var now = DateTime.Now;
        var currentDay = ClassroomService.GetCurrentDayOfWeekEnum(now.DayOfWeek);
        if (currentDay == null)
            return Enumerable.Empty<ScheduleEntryDto>();

        var weekNumber = System.Globalization.CultureInfo.CurrentCulture.Calendar
            .GetWeekOfYear(now, System.Globalization.CalendarWeekRule.FirstFourDayWeek, System.DayOfWeek.Monday);
        var currentWeekType = weekNumber % 2 == 1 ? WeekType.Numerator : WeekType.Denominator;

        var todayEntries = entries.Where(e =>
            e.DayOfWeek == currentDay.Value &&
            (e.WeekType == WeekType.Both || e.WeekType == currentWeekType));

        return _mapper.Map<IEnumerable<ScheduleEntryDto>>(todayEntries);
    }
}
