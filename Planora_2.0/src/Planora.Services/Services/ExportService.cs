using ClosedXML.Excel;
using Planora.Domain.Enums;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Planora.Services.Services;

public class ExportService : IExportService
{
    public ExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

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

    public byte[] ExportScheduleToPdf(IEnumerable<ScheduleEntryDto> entries, string title)
    {
        var sortedEntries = entries
            .OrderBy(e => e.DayOfWeek)
            .ThenBy(e => e.TimeSlotNumber)
            .ToList();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                
                // Fallback to basic fonts if Arial isn't available
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                page.Header().Text(title).SemiBold().FontSize(18).FontColor(Colors.Indigo.Darken2).AlignCenter();

                page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(80); // Day
                        columns.ConstantColumn(40); // Slot
                        columns.ConstantColumn(45); // Start
                        columns.ConstantColumn(45); // End
                        columns.RelativeColumn();   // Subject
                        columns.RelativeColumn();   // Type
                        columns.RelativeColumn();   // Teacher
                        columns.ConstantColumn(60); // Classroom
                        columns.RelativeColumn();   // Group
                    });

                    table.Header(header =>
                    {
                        var headers = new[] { "День", "Пара", "Поч.", "Кін.", "Предмет", "Тип", "Викладач", "Ауд.", "Група" };
                        foreach (var hdr in headers)
                        {
                            header.Cell().Background(Colors.Indigo.Lighten1).Padding(4).Text(hdr).FontColor(Colors.White).SemiBold().AlignCenter();
                        }
                    });

                    var isEven = false;
                    foreach (var entry in sortedEntries)
                    {
                        var bgColor = isEven ? Colors.Indigo.Lighten5 : Colors.White;
                        isEven = !isEven;

                        table.Cell().Background(bgColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(GetDayName(entry.DayOfWeek)).AlignCenter();
                        table.Cell().Background(bgColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(entry.TimeSlotNumber.ToString()).AlignCenter();
                        table.Cell().Background(bgColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(entry.StartTime.ToString(@"hh\:mm")).AlignCenter();
                        table.Cell().Background(bgColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(entry.EndTime.ToString(@"hh\:mm")).AlignCenter();
                        table.Cell().Background(bgColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(entry.SubjectName);
                        table.Cell().Background(bgColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(GetLessonTypeName(entry.LessonType));
                        table.Cell().Background(bgColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(entry.TeacherName);
                        table.Cell().Background(bgColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(entry.ClassroomNumber).AlignCenter();
                        table.Cell().Background(bgColor).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(entry.GroupName).AlignCenter();
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Згенеровано: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + " | Сторінка ");
                    x.CurrentPageNumber();
                    x.Span(" з ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
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
