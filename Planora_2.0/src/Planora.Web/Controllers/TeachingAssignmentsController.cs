using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;
using Planora.Domain.Entities;
using Planora.Domain.Enums;

namespace Planora.Web.Controllers;

[Authorize(Roles = "Admin")]
public class TeachingAssignmentsController : Controller
{
    private readonly ITeachingAssignmentService _assignmentService;
    private readonly ISubjectService _subjectService;
    private readonly UserManager<User> _userManager;

    public TeachingAssignmentsController(
        ITeachingAssignmentService assignmentService,
        ISubjectService subjectService,
        UserManager<User> userManager)
    {
        _assignmentService = assignmentService;
        _subjectService = subjectService;
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
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns();
            return View(dto);
        }

        await _assignmentService.CreateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var assignment = await _assignmentService.GetByIdAsync(id);
        if (assignment == null) return NotFound();

        await PopulateDropdowns();
        var dto = new UpdateTeachingAssignmentDto
        {
            Id = assignment.Id,
            TeacherId = assignment.TeacherId,
            SubjectId = assignment.SubjectId,
            HoursPerWeek = assignment.HoursPerWeek
        };
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateTeachingAssignmentDto dto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns();
            return View(dto);
        }

        await _assignmentService.UpdateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _assignmentService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdowns()
    {
        var teachers = _userManager.Users
            .Where(u => u.Role == UserRole.Teacher)
            .Select(u => new { u.Id, u.FullName })
            .ToList();

        var subjects = await _subjectService.GetAllAsync();

        ViewBag.Teachers = new SelectList(teachers, "Id", "FullName");
        ViewBag.Subjects = new SelectList(subjects, "Id", "Name");
    }
}
