using AutoMapper;
using Planora.Services.DTOs;
using Planora.Services.Interfaces;
using Planora.Services.Services.Interfaces;
using Planora.Domain.Entities;

namespace Planora.Services.Services;

public class TeachingAssignmentService : ITeachingAssignmentService
{
    private readonly ITeachingAssignmentRepository _repository;
    private readonly IMapper _mapper;

    public TeachingAssignmentService(ITeachingAssignmentRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TeachingAssignmentDto>> GetAllAsync()
    {
        var assignments = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<TeachingAssignmentDto>>(assignments);
    }

    public async Task<TeachingAssignmentDto?> GetByIdAsync(int id)
    {
        var assignment = await _repository.GetByIdAsync(id);
        return assignment == null ? null : _mapper.Map<TeachingAssignmentDto>(assignment);
    }

    public async Task<IEnumerable<TeachingAssignmentDto>> GetByTeacherIdAsync(string teacherId)
    {
        var assignments = await _repository.GetByTeacherIdAsync(teacherId);
        return _mapper.Map<IEnumerable<TeachingAssignmentDto>>(assignments);
    }

    public async Task<TeachingAssignmentDto> CreateAsync(CreateTeachingAssignmentDto dto)
    {
        var assignment = _mapper.Map<TeachingAssignment>(dto);
        var created = await _repository.AddAsync(assignment);
        return _mapper.Map<TeachingAssignmentDto>(created);
    }

    public async Task UpdateAsync(UpdateTeachingAssignmentDto dto)
    {
        var existing = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"TeachingAssignment with Id {dto.Id} not found.");

        _mapper.Map(dto, existing);
        await _repository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
