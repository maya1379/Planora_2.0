using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Planora.Domain.Entities;
using Planora.Domain.Enums;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;
using Planora.Web.ViewModels;

namespace Planora.Web.Controllers;

[Authorize(Roles = "Admin")]
public class GroupsController : Controller
{
    private readonly IGroupService _groupService;
    private readonly UserManager<User> _userManager;

    public GroupsController(IGroupService groupService, UserManager<User> userManager)
    {
        _groupService = groupService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var groups = await _groupService.GetAllAsync();
        return View(groups);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateGroupDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        await _groupService.CreateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var groups = await _groupService.GetByIdAsync(id);
        if (groups == null) return NotFound();

        return View(new UpdateGroupDto
        {
            Id = groups.Id,
            Name = groups.Name,
            Faculty = groups.Faculty,
            StudentCount = groups.StudentCount
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateGroupDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        await _groupService.UpdateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var group = await _groupService.GetByIdAsync(id);
        if (group == null) return NotFound();

        var students = _userManager.Users
            .Where(u => u.Role == UserRole.Student && u.GroupId == id)
            .OrderBy(u => u.FullName)
            .ToList();

        ViewBag.Students = students;
        return View(group);
    }

    [HttpGet]
    public async Task<IActionResult> AddStudents(int id)
    {
        var group = await _groupService.GetByIdAsync(id);
        if (group == null) return NotFound();

        var availableStudents = _userManager.Users
            .Where(u => u.Role == UserRole.Student && u.GroupId != id)
            .OrderBy(u => u.GroupId == null ? 0 : 1)
            .ThenBy(u => u.FullName)
            .ToList();

        var allGroups = await _groupService.GetAllAsync();
        ViewBag.AllGroups = allGroups.ToList();
        ViewBag.GroupName = group.Name;
        ViewBag.GroupId = id;
        return View(availableStudents);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddStudents(int groupId, List<string> studentIds)
    {
        if (studentIds == null || !studentIds.Any())
            return RedirectToAction(nameof(Details), new { id = groupId });

        foreach (var studentId in studentIds)
        {
            var student = await _userManager.FindByIdAsync(studentId);
            if (student != null)
            {
                student.GroupId = groupId;
                await _userManager.UpdateAsync(student);
            }
        }

        return RedirectToAction(nameof(Details), new { id = groupId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveStudent(int groupId, string studentId)
    {
        var student = await _userManager.FindByIdAsync(studentId);
        if (student != null && student.GroupId == groupId)
        {
            student.GroupId = null;
            await _userManager.UpdateAsync(student);
        }

        return RedirectToAction(nameof(Details), new { id = groupId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _groupService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
