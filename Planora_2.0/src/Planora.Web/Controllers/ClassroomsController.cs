using Planora.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;

namespace Planora.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class ClassroomsController : Controller
{
    private readonly IClassroomService _classroomService;
    private readonly IBuildingService _buildingService;

    public ClassroomsController(IClassroomService classroomService, IBuildingService buildingService)
    {
        _classroomService = classroomService;
        _buildingService = buildingService;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(int? buildingId = null)
    {
        var classrooms = await _classroomService.GetAllAsync();
        if (buildingId.HasValue)
        {
            classrooms = classrooms.Where(c => c.BuildingId == buildingId.Value);
        }
        return View(classrooms);
    }

    public async Task<IActionResult> AdminIndex(int? buildingId = null)
    {
        var classrooms = await _classroomService.GetAllAsync();
        if (buildingId.HasValue)
        {
            classrooms = classrooms.Where(c => c.BuildingId == buildingId.Value);
        }
        return View(classrooms);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateBuildingsDropdown();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateClassroomDto dto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateBuildingsDropdown();
            return View(dto);
        }

        await _classroomService.CreateAsync(dto);
        return RedirectToAction(nameof(AdminIndex));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var classrooms = await _classroomService.GetByIdAsync(id);
        if (classrooms == null) return NotFound();

        await PopulateBuildingsDropdown(classrooms.BuildingId);

        return View(new UpdateClassroomDto
        {
            Id = classrooms.Id,
            Number = classrooms.Number,
            Capacity = classrooms.Capacity,
            HasComputers = classrooms.HasComputers,
            HasProjector = classrooms.HasProjector,
            Faculty = classrooms.Faculty,
            BuildingId = classrooms.BuildingId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateClassroomDto dto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateBuildingsDropdown(dto.BuildingId);
            return View(dto);
        }

        await _classroomService.UpdateAsync(dto);
        return RedirectToAction(nameof(AdminIndex));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _classroomService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Аудиторію успішно видалено.";
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            TempData["ErrorMessage"] = "Неможливо видалити цю аудиторію, оскільки вона використовується в розкладі.";
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Сталася непередбачена помилка під час видалення аудиторії.";
        }
        return RedirectToAction(nameof(AdminIndex));
    }

    [Authorize] 
    public async Task<IActionResult> FreeRooms()
    {
        var freeRooms = await _classroomService.FindFreeClassroomsNowAsync();
        return View(freeRooms);
    }

    private async Task PopulateBuildingsDropdown(int? selectedBuildingId = null)
    {
        var buildings = await _buildingService.GetAllAsync();
        ViewBag.Buildings = new SelectList(buildings, "Id", "Name", selectedBuildingId);
    }
}
