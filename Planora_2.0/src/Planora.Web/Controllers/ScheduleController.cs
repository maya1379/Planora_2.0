using Planora.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Planora.Services.Services.Interfaces;
using Planora.Domain.Entities;
using Planora.Domain.Enums;
using Planora.Services.DTOs;
using Planora.Web.ViewModels;

namespace Planora.Web.Controllers;

[Authorize]
public class ScheduleController : Controller
{
    private readonly IScheduleService _scheduleService;
    private readonly IGroupService _groupService;
    private readonly IExportService _exportService;
    private readonly IClassroomService _classroomService;
    private readonly ISubjectService _subjectService;
    private readonly ITimeSlotService _timeSlotService;
    private readonly UserManager<User> _userManager;

    public ScheduleController(
        IScheduleService scheduleService,
        IGroupService groupService,
        IExportService exportService,
        IClassroomService classroomService,
        ISubjectService subjectService,
        ITimeSlotService timeSlotService,
        UserManager<User> userManager)
    {
        _scheduleService = scheduleService;
        _groupService = groupService;
        _exportService = exportService;
        _classroomService = classroomService;
        _subjectService = subjectService;
        _timeSlotService = timeSlotService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int? groupId, string? teacherId, bool editMode = false)
    {
        var groups = await _groupService.GetAllAsync();
        var teachersInRole = await _userManager.GetUsersInRoleAsync(AppRoles.Teacher);
        var teachers = teachersInRole
            .Select(u => new { u.Id, u.FullName })
            .ToList();

        ViewBag.Groups = new SelectList(groups, "Id", "Name", groupId);
        ViewBag.Teachers = new SelectList(teachers, "Id", "FullName", teacherId);
        ViewBag.EditMode = editMode;

        if (groupId.HasValue)
        {
            var entries = await _scheduleService.GetByGroupIdAsync(groupId.Value);
            return View(entries);
        }

        if (!string.IsNullOrEmpty(teacherId))
        {
            var entries = await _scheduleService.GetByTeacherIdAsync(teacherId);
            return View(entries);
        }

        var allEntries = await _scheduleService.GetAllAsync();
        return View(allEntries);
    }

    public async Task<IActionResult> Today(int? groupId, string? teacherId)
    {
        var groups = await _groupService.GetAllAsync();
        var teachersInRole = await _userManager.GetUsersInRoleAsync(AppRoles.Teacher);
        var teachers = teachersInRole
            .Select(u => new { u.Id, u.FullName })
            .ToList();

        ViewBag.Groups = new SelectList(groups, "Id", "Name", groupId);
        ViewBag.Teachers = new SelectList(teachers, "Id", "FullName", teacherId);

        if (groupId.HasValue)
        {
            var entries = await _scheduleService.GetTodayByGroupIdAsync(groupId.Value);
            return View(entries);
        }

        if (!string.IsNullOrEmpty(teacherId))
        {
            var entries = await _scheduleService.GetTodayByTeacherIdAsync(teacherId);
            return View(entries);
        }

        return View(Enumerable.Empty<ScheduleEntryDto>());
    }

    public async Task<IActionResult> TeacherLocation(string? teacherId)
    {
        var teachersInRole = await _userManager.GetUsersInRoleAsync(AppRoles.Teacher);
        var teachers = teachersInRole
            .Select(u => new { u.Id, u.FullName })
            .ToList();

        ViewBag.Teachers = new SelectList(teachers, "Id", "FullName", teacherId);

        if (!string.IsNullOrEmpty(teacherId))
        {
            var location = await _scheduleService.FindTeacherLocationAsync(teacherId);
            return View(location);
        }

        return View();
    }

    public async Task<IActionResult> SearchTeachers(string? query)
    {
        ViewBag.Query = query;

        if (!string.IsNullOrWhiteSpace(query))
        {
            var results = await _scheduleService.SearchTeachersAsync(query);
            return View(results);
        }

        return View(Enumerable.Empty<TeacherSearchDto>());
    }

    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> EditEntry(int id, int? filterGroupId, string? filterTeacherId)
    {
        var entry = await _scheduleService.GetByIdAsync(id);
        if (entry == null) return NotFound();

        await PrepareEditDropdowns();

        ViewBag.FilterGroupId = filterGroupId;
        ViewBag.FilterTeacherId = filterTeacherId;

        var model = new EditScheduleEntryViewModel
        {
            Id = entry.Id,
            DayOfWeek = entry.DayOfWeek,
            TimeSlotId = entry.TimeSlotId,
            ClassroomId = entry.ClassroomId,
            TeacherId = entry.TeacherId,
            SubjectId = entry.SubjectId,
            GroupId = entry.GroupId,
            WeekType = entry.WeekType
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> EditEntry(EditScheduleEntryViewModel model, int? filterGroupId, string? filterTeacherId)
    {
        if (!ModelState.IsValid)
        {
            await PrepareEditDropdowns();
            ViewBag.FilterGroupId = filterGroupId;
            ViewBag.FilterTeacherId = filterTeacherId;
            return View(model);
        }

        var dto = new CreateScheduleEntryDto
        {
            DayOfWeek = model.DayOfWeek,
            WeekType = model.WeekType,
            TimeSlotId = model.TimeSlotId,
            ClassroomId = model.ClassroomId,
            TeacherId = model.TeacherId,
            SubjectId = model.SubjectId,
            GroupId = model.GroupId
        };

        await _scheduleService.UpdateAsync(model.Id, dto);
        return RedirectToAction(nameof(Index), new { editMode = true, groupId = filterGroupId, teacherId = filterTeacherId });
    }

    public async Task<IActionResult> ExportExcel(int? groupId, string? teacherId)
    {
        IEnumerable<ScheduleEntryDto> entries;
        var title = "Розклад занять";

        if (groupId.HasValue)
        {
            entries = await _scheduleService.GetByGroupIdAsync(groupId.Value);
            var groups = await _groupService.GetAllAsync();
            var groupName = groups.FirstOrDefault(g => g.Id == groupId.Value)?.Name;
            title = $"Розклад — {groupName ?? "Група"}";
        }
        else if (!string.IsNullOrEmpty(teacherId))
        {
            entries = await _scheduleService.GetByTeacherIdAsync(teacherId);
            var teacher = _userManager.Users.FirstOrDefault(u => u.Id == teacherId);
            title = $"Розклад — {teacher?.FullName ?? "Викладач"}";
        }
        else
        {
            entries = await _scheduleService.GetAllAsync();
            title = "Розклад — Усі заняття";
        }

        var fileBytes = _exportService.ExportScheduleToExcel(entries, title);
        var fileName = $"Rozklad_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> DeleteEntry(int id, int? filterGroupId, string? filterTeacherId)
    {
        await _scheduleService.DeleteAsync(id);
        return RedirectToAction(nameof(Index), new { editMode = true, groupId = filterGroupId, teacherId = filterTeacherId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> ClearSchedule()
    {
        await _scheduleService.DeleteAllAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task PrepareEditDropdowns()
    {
        var groups = await _groupService.GetAllAsync();
        var classrooms = await _classroomService.GetAllAsync();
        var subjects = await _subjectService.GetAllAsync();
        var timeSlots = await _timeSlotService.GetAllAsync();
        var teachersInRole = await _userManager.GetUsersInRoleAsync(AppRoles.Teacher);
        var teachers = teachersInRole
            .Select(u => new { u.Id, u.FullName })
            .ToList();

        ViewBag.GroupsList = new SelectList(groups, "Id", "Name");
        ViewBag.ClassroomsList = new SelectList(classrooms, "Id", "Number");
        ViewBag.SubjectsList = new SelectList(subjects, "Id", "Name");
        ViewBag.TimeSlotsList = new SelectList(timeSlots, "Id", "Number");
        ViewBag.TeachersList = new SelectList(teachers, "Id", "FullName");

        ViewBag.DaysList = Enum.GetValues<DayOfWeekEnum>()
            .Where(d => d >= DayOfWeekEnum.Monday && d <= DayOfWeekEnum.Saturday)
            .Select(d => new SelectListItem
            {
                Value = ((int)d).ToString(),
                Text = d switch
                {
                    DayOfWeekEnum.Monday => "Понеділок",
                    DayOfWeekEnum.Tuesday => "Вівторок",
                    DayOfWeekEnum.Wednesday => "Середа",
                    DayOfWeekEnum.Thursday => "Четвер",
                    DayOfWeekEnum.Friday => "П'ятниця",
                    DayOfWeekEnum.Saturday => "Субота",
                    _ => d.ToString()
                }
            }).ToList();

        ViewBag.WeekTypesList = Enum.GetValues<WeekType>()
            .Select(w => new SelectListItem
            {
                Value = ((int)w).ToString(),
                Text = w switch
                {
                    WeekType.Both => "Обидва",
                    WeekType.Numerator => "Чисельник",
                    WeekType.Denominator => "Знаменник",
                    _ => w.ToString()
                }
            }).ToList();
    }
}
