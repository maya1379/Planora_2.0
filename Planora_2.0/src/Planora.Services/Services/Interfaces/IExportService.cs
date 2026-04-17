using Planora.Services.DTOs;

namespace Planora.Services.Services.Interfaces;

public interface IExportService
{
    byte[] ExportScheduleToExcel(IEnumerable<ScheduleEntryDto> entries, string title);
    byte[] ExportScheduleToPdf(IEnumerable<ScheduleEntryDto> entries, string title);
}
