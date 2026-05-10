using AutoMapper;
using Planora.Domain.Entities;
using Planora.Services.DTOs;
using Planora.Services.Interfaces;
using Planora.Services.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planora.Services.Services;

public class ScheduleNoteService : IScheduleNoteService
{
    private readonly IScheduleNoteRepository _repository;
    private readonly IMapper _mapper;

    public ScheduleNoteService(IScheduleNoteRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ScheduleNoteDto>> GetNotesForScheduleAsync(int scheduleId, string userId)
    {
        var notes = await _repository.GetNotesForScheduleAsync(scheduleId, userId);

        return notes.Select(n => new ScheduleNoteDto
        {
            Id = n.Id,
            ScheduleId = n.ScheduleId,
            UserId = n.UserId,
            Content = n.Content,
            CreatedAt = n.CreatedAt
        });
    }

    public async Task<ScheduleNoteDto> CreateNoteAsync(CreateScheduleNoteDto dto, string userId)
    {
        var note = new ScheduleNote
        {
            ScheduleId = dto.ScheduleId,
            UserId = userId,
            Content = dto.Content,
            CreatedAt = System.DateTime.UtcNow
        };

        var createdNote = await _repository.AddAsync(note);

        return new ScheduleNoteDto
        {
            Id = createdNote.Id,
            ScheduleId = createdNote.ScheduleId,
            UserId = createdNote.UserId,
            Content = createdNote.Content,
            CreatedAt = createdNote.CreatedAt
        };
    }

    public async Task DeleteNoteAsync(int noteId, string userId)
    {
        var note = await _repository.GetByIdAsync(noteId);
        if (note != null && note.UserId == userId)
        {
            await _repository.DeleteAsync(note);
        }
    }
}
