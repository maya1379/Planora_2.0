using AutoMapper;
using Planora.Services.DTOs;
using Planora.Services.Interfaces;
using Planora.Services.Services.Interfaces;
using Planora.Domain.Entities;

namespace Planora.Services.Services;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _repository;
    private readonly IMapper _mapper;

    public SubjectService(ISubjectRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SubjectDto>> GetAllAsync()
    {
        var subjects = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<SubjectDto>>(subjects);
    }

    public async Task<SubjectDto?> GetByIdAsync(int id)
    {
        var subjects = await _repository.GetByIdAsync(id);
        return subjects == null ? null : _mapper.Map<SubjectDto>(subjects);
    }

    public async Task<SubjectDto> CreateAsync(CreateSubjectDto dto)
    {
        var subjects = _mapper.Map<Subjects>(dto);
        var created = await _repository.AddAsync(subjects);
        return _mapper.Map<SubjectDto>(created);
    }

    public async Task UpdateAsync(UpdateSubjectDto dto)
    {
        var existing = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Subjects with Id {dto.Id} not found.");

        _mapper.Map(dto, existing);
        await _repository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
