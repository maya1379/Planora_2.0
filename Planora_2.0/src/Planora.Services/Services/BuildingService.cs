using AutoMapper;
using Planora.Services.DTOs;
using Planora.Services.Interfaces;
using Planora.Services.Services.Interfaces;
using Planora.Domain.Entities;

namespace Planora.Services.Services;

public class BuildingService : IBuildingService
{
    private readonly IBuildingRepository _repository;
    private readonly IMapper _mapper;

    public BuildingService(IBuildingRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BuildingDto>> GetAllAsync()
    {
        var buildings = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<BuildingDto>>(buildings);
    }

    public async Task<BuildingDto?> GetByIdAsync(int id)
    {
        var building = await _repository.GetByIdAsync(id);
        return building == null ? null : _mapper.Map<BuildingDto>(building);
    }

    public async Task<BuildingDto> CreateAsync(CreateBuildingDto dto)
    {
        var building = _mapper.Map<Building>(dto);
        var created = await _repository.AddAsync(building);
        return _mapper.Map<BuildingDto>(created);
    }

    public async Task UpdateAsync(UpdateBuildingDto dto)
    {
        var existing = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Building with Id {dto.Id} not found.");

        _mapper.Map(dto, existing);
        await _repository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
    public async Task<BuildingStatisticsDto?> GetStatisticsAsync(int id)
{
    // Отримуємо корпус через репозиторій з підвантаженими даними
    var building = await _repository.GetWithClassroomsAndSchedulesAsync(id);
    
    if (building == null) return null;

    return new BuildingStatisticsDto
    {
        BuildingId = building.Id,
        Name = building.Name,
        ClassroomsCount = building.Classrooms.Count,
        // Рахуємо суму всіх пар у всіх аудиторіях цього корпусу
        TotalSchedulesCount = building.Classrooms.Sum(c => c.Schedules.Count)
    };
}
}
