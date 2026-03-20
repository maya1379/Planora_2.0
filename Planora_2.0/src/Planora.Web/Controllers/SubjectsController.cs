using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;

namespace Planora.Web.Controllers;

[Authorize(Roles = "Admin")]
public class SubjectsController : Controller
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    public async Task<IActionResult> Index()
    {
        var subjects = await _subjectService.GetAllAsync();
        return View(subjects);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSubjectDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        await _subjectService.CreateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var subjects = await _subjectService.GetByIdAsync(id);
        if (subjects == null) return NotFound();

        return View(new UpdateSubjectDto
        {
            Id = subjects.Id,
            Name = subjects.Name,
            Type = subjects.Type
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateSubjectDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        await _subjectService.UpdateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _subjectService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
