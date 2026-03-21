using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Planora.Domain.Entities;
using Planora.Domain.Enums;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;

namespace Planora.Web.Controllers;

[Authorize(Roles = "Admin")]
public class TeachingAssignmentsController : Controller
{
    private readonly ITeachingAssignmentService _assignmentService;
    private readonly ISubjectService _subjectService;
    private readonly IGroupService _groupService;
    private readonly UserManager<User> _userManager;

    public TeachingAssignmentsController(
        ITeachingAssignmentService assignmentService,
        ISubjectService subjectService,
        IGroupService groupService,
        UserManager<User> userManager)
    {
        _assignmentService = assignmentService;
        _subjectService = subjectService;
        _groupService = groupService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var assignments = await _assignmentService.GetAllAsync();
        return View(assignments);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDropdowns();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTeachingAssignmentDto dto)
    {
        if (ModelState.IsValid)
        {
            await _assignmentService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        await PopulateDropdowns();
        return View(dto);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var assignment = await _assignmentService.GetByIdAsync(id);
        if (assignment == null) return NotFound();

        var dto = new UpdateTeachingAssignmentDto
        {
            Id = assignment.Id,
            TeacherId = assignment.TeacherId,
            SubjectId = assignment.SubjectId,
            GroupId = assignment.GroupId,
            HoursPerWeek = assignment.HoursPerWeek
        };
        await PopulateDropdowns();
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateTeachingAssignmentDto dto)
    {
        if (ModelState.IsValid)
        {
            await _assignmentService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        await PopulateDropdowns();
        return View(dto);
    }

    public async Task<IActionResult> Delete(int id)
    {
        await _assignmentService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdowns()
    {
        var teachers = _userManager.Users
            .Where(u => u.Role == UserRole.Teacher)
            .OrderBy(u => u.FullName)
            .Select(u => new { u.Id, u.FullName })
            .ToList();

        var subjects = await _subjectService.GetAllAsync();
        var groups = await _groupService.GetAllAsync();

        ViewBag.Teachers = new SelectList(teachers, "Id", "FullName");
        ViewBag.Subjects = new SelectList(subjects, "Id", "Name");
        ViewBag.Groups = new SelectList(groups, "Id", "Name");
    }
}
