using AutoMapper;
using Planora.Services.DTOs;
using Planora.Services.Interfaces;
using Planora.Services.Services.Interfaces;
using Planora.Domain.Entities;

namespace Planora.Services.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _repository;
    private readonly IMapper _mapper;

    public GroupService(IGroupRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GroupDto>> GetAllAsync()
    {
        var groups = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<GroupDto>>(groups);
    }

    public async Task<GroupDto?> GetByIdAsync(int id)
    {
        var groups = await _repository.GetByIdAsync(id);
        return groups == null ? null : _mapper.Map<GroupDto>(groups);
    }

    public async Task<IEnumerable<GroupDto>> GetByFacultyAsync(string faculty)
    {
        var groups = await _repository.GetByFacultyAsync(faculty);
        return _mapper.Map<IEnumerable<GroupDto>>(groups);
    }

    public async Task<GroupDto> CreateAsync(CreateGroupDto dto)
    {
        var groups = _mapper.Map<Groups>(dto);
        var created = await _repository.AddAsync(groups);
        return _mapper.Map<GroupDto>(created);
    }

    public async Task UpdateAsync(UpdateGroupDto dto)
    {
        var existing = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Groups with Id {dto.Id} not found.");

        _mapper.Map(dto, existing);
        await _repository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
