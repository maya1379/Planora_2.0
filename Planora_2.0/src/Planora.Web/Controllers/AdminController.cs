using Planora.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Planora.Domain.Entities;
using Planora.Services.Services.Interfaces;
using Planora.Web.ViewModels;

namespace Planora.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class AdminController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IBuildingService _buildingService;
    private readonly IGroupService _groupService;
    private readonly IScheduleService _scheduleService;
    private readonly ISubjectService _subjectService;

    public AdminController(
        UserManager<User> userManager,
        IBuildingService buildingService,
        IGroupService groupService,
        IScheduleService scheduleService,
        ISubjectService subjectService)
    {
        _userManager = userManager;
        _buildingService = buildingService;
        _groupService = groupService;
        _scheduleService = scheduleService;
        _subjectService = subjectService;
    }

    public async Task<IActionResult> Index()
    {
        var allUsers = _userManager.Users.ToList();
        var students = await _userManager.GetUsersInRoleAsync(AppRoles.Student);
        var teachers = await _userManager.GetUsersInRoleAsync(AppRoles.Teacher);
        var allGroups = await _groupService.GetAllAsync();
        var allBuildings = await _buildingService.GetAllAsync();
        var allScheduleChunks = await _scheduleService.GetAllAsync();
        var allSubjects = await _subjectService.GetAllAsync();

        var vm = new AdminDashboardViewModel
        {
            TotalStudents = students.Count,
            TotalTeachers = teachers.Count,
            TotalGroups = allGroups.Count(),
            TotalBuildings = allBuildings.Count(),
            TotalScheduleEntries = allScheduleChunks.Count(),
            TotalSubjects = allSubjects.Count(),
            RecentUsers = allUsers
                .OrderByDescending(u => u.Id)
                .Take(5)
                .ToList()
        };

        return View(vm);
    }
}
