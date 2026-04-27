using Planora.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;
using Planora.Web.ViewModels;

namespace Planora.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
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
        try
        {
            await _buildingService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Корпус успішно видалено.";
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            TempData["ErrorMessage"] = "Неможливо видалити цей корпус, оскільки він має прив'язані аудиторії.";
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Сталася непередбачена помилка під час видалення корпусу.";
        }
        return RedirectToAction(nameof(Index));
    }
    [HttpGet("{id}/statistics")]
    public async Task<ActionResult<BuildingStatisticsDto>> GetStatistics(int id)
    {
        var stats = await _buildingService.GetStatisticsAsync(id);

        if (stats == null)
        {
            return NotFound($"Building with ID {id} not found.");
        }
        return Ok(stats);
    }
    [HttpGet]
    public async Task<IActionResult> Statistics(int id)
    {
        var stats = await _buildingService.GetStatisticsAsync(id);
        if (stats == null) return NotFound();

        var viewModel = new BuildingStatisticsViewModel
        {
            Name = stats.Name,
            ClassroomsCount = stats.ClassroomsCount,
            TotalSchedulesCount = stats.TotalSchedulesCount
        };

        return View(viewModel); 
}
}
