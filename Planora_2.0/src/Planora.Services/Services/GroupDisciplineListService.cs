using AutoMapper;
using Planora.Services.DTOs;
using Planora.Services.Interfaces;
using Planora.Services.Services.Interfaces;
using Planora.Domain.Entities;

namespace Planora.Services.Services;

public class GroupSubjectService : IGroupSubjectService
{
    private readonly IGroupSubjectRepository _repository;
    private readonly IMapper _mapper;

    public GroupSubjectService(IGroupSubjectRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GroupSubjectDto>> GetAllAsync()
    {
        var groupDisciplineLists = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<GroupSubjectDto>>(groupDisciplineLists);
    }

    public async Task<GroupSubjectDto?> GetByIdAsync(int id)
    {
        var groupDisciplineList = await _repository.GetByIdAsync(id);
        return groupDisciplineList == null ? null : _mapper.Map<GroupSubjectDto>(groupDisciplineList);
    }

    public async Task<IEnumerable<GroupSubjectDto>> GetByGroupIdAsync(int groupId)
    {
        var groupDisciplineLists = await _repository.GetByGroupIdAsync(groupId);
        return _mapper.Map<IEnumerable<GroupSubjectDto>>(groupDisciplineLists);
    }

    public async Task<GroupSubjectDto> CreateAsync(CreateGroupSubjectDto dto)
    {
        var groupDisciplineList = _mapper.Map<GroupDisciplineList>(dto);
        var created = await _repository.AddAsync(groupDisciplineList);
        return _mapper.Map<GroupSubjectDto>(created);
    }

    public async Task UpdateAsync(UpdateGroupSubjectDto dto)
    {
        var existing = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"GroupDisciplineList with Id {dto.Id} not found.");

        _mapper.Map(dto, existing);
        await _repository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
