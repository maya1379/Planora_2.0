using ClosedXML.Excel;
using Planora.Domain.Enums;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;

namespace Planora.Services.Services;

public class ExportService : IExportService
{
    public byte[] ExportScheduleToExcel(IEnumerable<ScheduleEntryDto> entries, string title)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Розклад");

        worksheet.Cell(1, 1).Value = title;
        worksheet.Range(1, 1, 1, 9).Merge();
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 14;
        worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        var headerRow = 3;
        var headers = new[] { "День", "Пара №", "Початок", "Кінець", "Предмет", "Тип", "Викладач", "Аудиторія", "Група" };

        for (int i = 0; i < headers.Length; i++)
        {
            var cell = worksheet.Cell(headerRow, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#6366f1");
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

        var sortedEntries = entries
            .OrderBy(e => e.DayOfWeek)
            .ThenBy(e => e.TimeSlotNumber)
            .ToList();

        var dataRow = headerRow + 1;
        foreach (var entry in sortedEntries)
        {
            worksheet.Cell(dataRow, 1).Value = GetDayName(entry.DayOfWeek);
            worksheet.Cell(dataRow, 2).Value = entry.TimeSlotNumber;
            worksheet.Cell(dataRow, 3).Value = entry.StartTime.ToString(@"hh\:mm");
            worksheet.Cell(dataRow, 4).Value = entry.EndTime.ToString(@"hh\:mm");
            worksheet.Cell(dataRow, 5).Value = entry.SubjectName;
            worksheet.Cell(dataRow, 6).Value = GetLessonTypeName(entry.LessonType);
            worksheet.Cell(dataRow, 7).Value = entry.TeacherName;
            worksheet.Cell(dataRow, 8).Value = entry.ClassroomNumber;
            worksheet.Cell(dataRow, 9).Value = entry.GroupName;

            for (int col = 1; col <= 9; col++)
            {
                worksheet.Cell(dataRow, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            if (dataRow % 2 == 0)
            {
                worksheet.Range(dataRow, 1, dataRow, 9).Style.Fill.BackgroundColor = XLColor.FromHtml("#f0f0ff");
            }

            dataRow++;
        }

        worksheet.Columns().AdjustToContents();

        worksheet.Cell(dataRow + 1, 1).Value = $"Згенеровано: {DateTime.Now:dd.MM.yyyy HH:mm}";
        worksheet.Cell(dataRow + 1, 1).Style.Font.Italic = true;
        worksheet.Cell(dataRow + 1, 1).Style.Font.FontColor = XLColor.Gray;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static string GetDayName(DayOfWeekEnum day) => day switch
    {
        DayOfWeekEnum.Monday => "Понеділок",
        DayOfWeekEnum.Tuesday => "Вівторок",
        DayOfWeekEnum.Wednesday => "Середа",
        DayOfWeekEnum.Thursday => "Четвер",
        DayOfWeekEnum.Friday => "П'ятниця",
        DayOfWeekEnum.Saturday => "Субота",
        _ => "Неділя"
    };

    private static string GetLessonTypeName(LessonType type) => type switch
    {
        LessonType.Lecture => "Лекція",
        LessonType.Practice => "Практика",
        LessonType.Lab => "Лабораторна",
        _ => type.ToString()
    };
}
