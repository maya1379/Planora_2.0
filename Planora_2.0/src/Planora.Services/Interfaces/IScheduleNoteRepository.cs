using Planora.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planora.Services.Interfaces;

public interface IScheduleNoteRepository
{
    Task<IEnumerable<ScheduleNote>> GetNotesForScheduleAsync(int scheduleId, string userId);
    Task<ScheduleNote> AddAsync(ScheduleNote note);
    Task<ScheduleNote?> GetByIdAsync(int id);
    Task DeleteAsync(ScheduleNote note);
}
