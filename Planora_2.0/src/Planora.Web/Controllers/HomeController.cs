using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Planora.Domain.Entities;
using Planora.Domain.Constants;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;

namespace Planora.Web.Controllers;

public class HomeController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IScheduleService _scheduleService;

    public HomeController(UserManager<User> userManager, IScheduleService scheduleService)
    {
        _userManager = userManager;
        _scheduleService = scheduleService;
    }

    public async Task<IActionResult> Index()
    {
        var model = new List<ScheduleEntryDto>();

        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var isAdmin = await _userManager.IsInRoleAsync(user, AppRoles.Admin);
                var isStudent = await _userManager.IsInRoleAsync(user, AppRoles.Student);
                var isTeacher = await _userManager.IsInRoleAsync(user, AppRoles.Teacher);

                if (isAdmin)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (isStudent && user.GroupId.HasValue)
                {
                    var studentSchedule = await _scheduleService.GetTodayByGroupIdAsync(user.GroupId.Value);
                    model = studentSchedule.ToList();
                }
                else if (isTeacher)
                {
                    var teacherSchedule = await _scheduleService.GetTodayByTeacherIdAsync(user.Id);
                    model = teacherSchedule.ToList();
                }
            }
        }

        return View(model);
    }

    public async Task<IActionResult> Schedule()
    {
        var model = new List<ScheduleEntryDto>();

        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var isStudent = await _userManager.IsInRoleAsync(user, AppRoles.Student);
                var isTeacher = await _userManager.IsInRoleAsync(user, AppRoles.Teacher);

                if (isStudent && user.GroupId.HasValue)
                {
                    var result = await _scheduleService.GetByGroupIdAsync(user.GroupId.Value);
                    model = result.ToList();
                }
                else if (isTeacher)
                {
                    var result = await _scheduleService.GetByTeacherIdAsync(user.Id);
                    model = result.ToList();
                }
            }
        }

        return View(model);
    }

    public IActionResult RoomSchedule()
    {
        return View();
    }

    public IActionResult FindRoom()
    {
        return View();
    }


    public IActionResult Error()
    {
        return View();
    }
}
