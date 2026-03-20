using AutoMapper;
using Planora.Services.DTOs;
using Planora.Services.Interfaces;
using Planora.Services.Services.Interfaces;
using Planora.Domain.Entities;
using Planora.Domain.Enums;

namespace Planora.Services.Services;

public class ClassroomService : IClassroomService
{
    private readonly IClassroomRepository _repository;
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IMapper _mapper;

    public ClassroomService(IClassroomRepository repository, ITimeSlotRepository timeSlotRepository, IMapper mapper)
    {
        _repository = repository;
        _timeSlotRepository = timeSlotRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ClassroomDto>> GetAllAsync()
    {
        var classrooms = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ClassroomDto>>(classrooms);
    }

    public async Task<ClassroomDto?> GetByIdAsync(int id)
    {
        var classrooms = await _repository.GetByIdAsync(id);
        return classrooms == null ? null : _mapper.Map<ClassroomDto>(classrooms);
    }

    public async Task<IEnumerable<ClassroomDto>> GetByBuildingIdAsync(int buildingId)
    {
        var classrooms = await _repository.GetByBuildingIdAsync(buildingId);
        return _mapper.Map<IEnumerable<ClassroomDto>>(classrooms);
    }

    public async Task<IEnumerable<ClassroomDto>> GetAvailableAsync(DayOfWeekEnum dayOfWeek, int timeSlotId, WeekType weekType)
    {
        var classrooms = await _repository.GetAvailableAsync(dayOfWeek, timeSlotId, weekType);
        return _mapper.Map<IEnumerable<ClassroomDto>>(classrooms);
    }

    public async Task<IEnumerable<ClassroomDto>> FindFreeClassroomsNowAsync()
    {
        var now = DateTime.Now;
        var currentDayOfWeek = GetCurrentDayOfWeekEnum(now.DayOfWeek);

        if (currentDayOfWeek == null)
        {

            var allClassrooms = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ClassroomDto>>(allClassrooms);
        }

        var timeSlots = await _timeSlotRepository.GetAllAsync();
        var currentTime = now.TimeOfDay;
        var currentTimeSlot = timeSlots.FirstOrDefault(ts =>
            ts.StartTime <= currentTime && ts.EndTime >= currentTime);

        if (currentTimeSlot == null)
        {

            var allClassrooms = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ClassroomDto>>(allClassrooms);
        }

        var weekNumber = System.Globalization.CultureInfo.CurrentCulture.Calendar
            .GetWeekOfYear(now, System.Globalization.CalendarWeekRule.FirstFourDayWeek, System.DayOfWeek.Monday);
        var weekType = weekNumber % 2 == 1 ? WeekType.Numerator : WeekType.Denominator;

        var available = await _repository.GetAvailableAsync(currentDayOfWeek.Value, currentTimeSlot.Id, weekType);
        return _mapper.Map<IEnumerable<ClassroomDto>>(available);
    }

    public async Task<ClassroomDto> CreateAsync(CreateClassroomDto dto)
    {
        var classrooms = _mapper.Map<Classrooms>(dto);
        var created = await _repository.AddAsync(classrooms);
        return _mapper.Map<ClassroomDto>(created);
    }

    public async Task UpdateAsync(UpdateClassroomDto dto)
    {
        var existing = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Classrooms with Id {dto.Id} not found.");

        _mapper.Map(dto, existing);
        await _repository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }

    public static DayOfWeekEnum? GetCurrentDayOfWeekEnum(System.DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            System.DayOfWeek.Monday => DayOfWeekEnum.Monday,
            System.DayOfWeek.Tuesday => DayOfWeekEnum.Tuesday,
            System.DayOfWeek.Wednesday => DayOfWeekEnum.Wednesday,
            System.DayOfWeek.Thursday => DayOfWeekEnum.Thursday,
            System.DayOfWeek.Friday => DayOfWeekEnum.Friday,
            System.DayOfWeek.Saturday => DayOfWeekEnum.Saturday,
            _ => null 
        };
    }
}
