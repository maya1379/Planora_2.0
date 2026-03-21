using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Planora.Domain.Entities;
using Planora.Services.Services.Interfaces;
using Planora.Web.ViewModels;

namespace Planora.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IBuildingService _buildingService;
    private readonly IGroupService _groupService;
    private readonly IScheduleService _scheduleService;

    public AdminController(
        UserManager<User> userManager,
        IBuildingService buildingService,
        IGroupService groupService,
        IScheduleService scheduleService)
    {
        _userManager = userManager;
        _buildingService = buildingService;
        _groupService = groupService;
        _scheduleService = scheduleService;
    }

    public async Task<IActionResult> Index()
    {
        var allUsers = _userManager.Users.ToList();
        var allGroups = await _groupService.GetAllAsync();
        var allBuildings = await _buildingService.GetAllAsync();
        var allScheduleChunks = await _scheduleService.GetAllAsync();

        var vm = new AdminDashboardViewModel
        {
            TotalStudents = allUsers.Count(u => u.Role == Domain.Enums.UserRole.Student),
            TotalTeachers = allUsers.Count(u => u.Role == Domain.Enums.UserRole.Teacher),
            TotalGroups = allGroups.Count(),
            TotalBuildings = allBuildings.Count(),
            TotalScheduleEntries = allScheduleChunks.Count(),
            RecentUsers = allUsers
                .OrderByDescending(u => u.Id)
                .Take(5)
                .ToList()
        };

        return View(vm);
    }
}
