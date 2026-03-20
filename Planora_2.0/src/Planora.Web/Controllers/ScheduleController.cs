using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Planora.Services.Services.Interfaces;
using Planora.Domain.Entities;
using Planora.Domain.Enums;

namespace Planora.Web.Controllers;

[Authorize]
public class ScheduleController : Controller
{
    private readonly IScheduleService _scheduleService;
    private readonly IGroupService _groupService;
    private readonly UserManager<User> _userManager;

    public ScheduleController(
        IScheduleService scheduleService,
        IGroupService groupService,
        UserManager<User> userManager)
    {
        _scheduleService = scheduleService;
        _groupService = groupService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int? groupId, string? teacherId)
    {
        var groups = await _groupService.GetAllAsync();
        var teachers = _userManager.Users
            .Where(u => u.Role == UserRole.Teacher)
            .Select(u => new { u.Id, u.FullName })
            .ToList();

        ViewBag.Groups = new SelectList(groups, "Id", "Name", groupId);
        ViewBag.Teachers = new SelectList(teachers, "Id", "FullName", teacherId);

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
        var teachers = _userManager.Users
            .Where(u => u.Role == UserRole.Teacher)
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

        return View(Enumerable.Empty<Planora.Services.DTOs.ScheduleEntryDto>());
    }

    public async Task<IActionResult> TeacherLocation(string? teacherId)
    {
        var teachers = _userManager.Users
            .Where(u => u.Role == UserRole.Teacher)
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

        return View(Enumerable.Empty<Planora.Services.DTOs.TeacherSearchDto>());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteEntry(int id)
    {
        await _scheduleService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ClearSchedule()
    {
        await _scheduleService.DeleteAllAsync();
        return RedirectToAction(nameof(Index));
    }
}
