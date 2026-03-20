using Planora.Services.DTOs;

namespace Planora.Services.Services.Interfaces;

public interface IScheduleGenerationService
{
    Task<ScheduleGenerationResultDto> GenerateScheduleAsync();
}
