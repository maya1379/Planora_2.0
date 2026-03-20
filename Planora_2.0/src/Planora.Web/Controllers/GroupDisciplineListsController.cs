using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;

namespace Planora.Web.Controllers;

[Authorize(Roles = "Admin")]
public class GroupSubjectsController : Controller
{
    private readonly IGroupSubjectService _groupSubjectService;
    private readonly IGroupService _groupService;
    private readonly ISubjectService _subjectService;

    public GroupSubjectsController(
        IGroupSubjectService groupSubjectService,
        IGroupService groupService,
        ISubjectService subjectService)
    {
        _groupSubjectService = groupSubjectService;
        _groupService = groupService;
        _subjectService = subjectService;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _groupSubjectService.GetAllAsync();
        return View(items);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDropdowns();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateGroupSubjectDto dto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns();
            return View(dto);
        }

        await _groupSubjectService.CreateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _groupSubjectService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdowns()
    {
        var groups = await _groupService.GetAllAsync();
        var subjects = await _subjectService.GetAllAsync();

        ViewBag.Groups = new SelectList(groups, "Id", "Name");
        ViewBag.Subjects = new SelectList(subjects, "Id", "Name");
    }
}
