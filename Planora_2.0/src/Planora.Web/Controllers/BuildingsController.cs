using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;

namespace Planora.Web.Controllers;

[Authorize(Roles = "Admin")]
public class BuildingsController : Controller
{
    private readonly IBuildingService _buildingService;

    public BuildingsController(IBuildingService buildingService)
    {
        _buildingService = buildingService;
    }

    public async Task<IActionResult> Index()
    {
        var buildings = await _buildingService.GetAllAsync();
        return NotFound();
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBuildingDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        await _buildingService.CreateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var building = await _buildingService.GetByIdAsync(id);
        if (building == null) return NotFound();

        return View(new UpdateBuildingDto
        {
            Id = building.Id,
            Name = building.Name,
            Address = building.Address
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateBuildingDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        await _buildingService.UpdateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _buildingService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
