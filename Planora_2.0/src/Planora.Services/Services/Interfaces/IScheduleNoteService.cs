using Planora.Services.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planora.Services.Services.Interfaces;

public interface IScheduleNoteService
{
    Task<IEnumerable<ScheduleNoteDto>> GetNotesForScheduleAsync(int scheduleId, string userId);
    Task<ScheduleNoteDto> CreateNoteAsync(CreateScheduleNoteDto dto, string userId);
    Task DeleteNoteAsync(int noteId, string userId);
}
