using AutoMapper;
using Planora.Services.DTOs;
using Planora.Services.Interfaces;
using Planora.Services.Services.Interfaces;
using Planora.Domain.Entities;

namespace Planora.Services.Services;

public class TimeSlotService : ITimeSlotService
{
    private readonly ITimeSlotRepository _repository;
    private readonly IMapper _mapper;

    public TimeSlotService(ITimeSlotRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TimeSlotDto>> GetAllAsync()
    {
        var timeSlots = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<TimeSlotDto>>(timeSlots);
    }

    public async Task<TimeSlotDto?> GetByIdAsync(int id)
    {
        var timeSlot = await _repository.GetByIdAsync(id);
        return timeSlot == null ? null : _mapper.Map<TimeSlotDto>(timeSlot);
    }

    public async Task<TimeSlotDto> CreateAsync(CreateTimeSlotDto dto)
    {
        var timeSlot = _mapper.Map<TimeSlot>(dto);
        var created = await _repository.AddAsync(timeSlot);
        return _mapper.Map<TimeSlotDto>(created);
    }

    public async Task UpdateAsync(TimeSlotDto dto)
    {
        var existing = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"TimeSlot with Id {dto.Id} not found.");

        existing.Number = dto.Number;
        existing.StartTime = dto.StartTime;
        existing.EndTime = dto.EndTime;
        await _repository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
