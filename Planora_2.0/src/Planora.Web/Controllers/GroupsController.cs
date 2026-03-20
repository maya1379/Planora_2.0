using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;

namespace Planora.Web.Controllers;

[Authorize(Roles = "Admin")]
public class GroupsController : Controller
{
    private readonly IGroupService _groupService;

    public GroupsController(IGroupService groupService)
    {
        _groupService = groupService;
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
        var groups = await _groupService.GetByIdAsync(id);
        if (groups == null) return NotFound();
        return View(groups);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _groupService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
