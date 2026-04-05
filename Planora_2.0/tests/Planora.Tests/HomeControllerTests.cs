using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Planora.Domain.Entities;
using Planora.Domain.Constants;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;
using Planora.Web.Controllers;
using Xunit;

namespace Planora.Tests;

public class HomeControllerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IScheduleService> _scheduleServiceMock;
    private readonly HomeController _controller;

    public HomeControllerTests()
    {
        var userStore = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStore.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);

        _scheduleServiceMock = new Mock<IScheduleService>();

        _controller = new HomeController(_userManagerMock.Object, _scheduleServiceMock.Object);
    }

    private void SetAuthenticatedUser()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, "1") }, "TestAuth"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task Index_WhenAnonymous_ReturnsEmptyViewModel()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<ScheduleEntryDto>>(viewResult.Model);
        Assert.Empty(model);
    }

    [Fact]
    public async Task Index_WhenAdmin_RedirectsToAdminIndex()
    {
        SetAuthenticatedUser();

        var user = new User { Id = "1" };
        _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.IsInRoleAsync(user, AppRoles.Admin)).ReturnsAsync(true);

        var result = await _controller.Index();

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Admin", redirectResult.ControllerName);
    }

    [Fact]
    public async Task Index_WhenStudentWithGroup_ReturnsStudentTodaySchedule()
    {
        SetAuthenticatedUser();

        var user = new User { Id = "1", GroupId = 5 };
        var schedule = new List<ScheduleEntryDto> { new ScheduleEntryDto { Id = 1 } };

        _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.IsInRoleAsync(user, AppRoles.Student)).ReturnsAsync(true);
        _scheduleServiceMock.Setup(s => s.GetTodayByGroupIdAsync(5)).ReturnsAsync(schedule);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<ScheduleEntryDto>>(viewResult.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task Index_WhenTeacher_ReturnsTeacherTodaySchedule()
    {
        SetAuthenticatedUser();

        var user = new User { Id = "teacher1" };
        var schedule = new List<ScheduleEntryDto> { new ScheduleEntryDto { Id = 2 } };

        _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.IsInRoleAsync(user, AppRoles.Teacher)).ReturnsAsync(true);
        _scheduleServiceMock.Setup(s => s.GetTodayByTeacherIdAsync("teacher1")).ReturnsAsync(schedule);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<ScheduleEntryDto>>(viewResult.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task Schedule_WhenStudent_ReturnsGroupSchedule()
    {
        SetAuthenticatedUser();

        var user = new User { Id = "1", GroupId = 2 };
        var schedule = new List<ScheduleEntryDto> { new ScheduleEntryDto { Id = 10 } };

        _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.IsInRoleAsync(user, AppRoles.Student)).ReturnsAsync(true);
        _scheduleServiceMock.Setup(s => s.GetByGroupIdAsync(2)).ReturnsAsync(schedule);

        var result = await _controller.Schedule();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<ScheduleEntryDto>>(viewResult.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task Schedule_WhenTeacher_ReturnsTeacherSchedule()
    {
        SetAuthenticatedUser();

        var user = new User { Id = "teacher1" };
        var schedule = new List<ScheduleEntryDto> { new ScheduleEntryDto { Id = 20 } };

        _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.IsInRoleAsync(user, AppRoles.Teacher)).ReturnsAsync(true);
        _scheduleServiceMock.Setup(s => s.GetByTeacherIdAsync("teacher1")).ReturnsAsync(schedule);

        var result = await _controller.Schedule();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<ScheduleEntryDto>>(viewResult.Model);
        Assert.Single(model);
    }

    [Fact]
    public void RoomSchedule_ReturnsView()
    {
        var result = _controller.RoomSchedule();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void FindRoom_ReturnsView()
    {
        var result = _controller.FindRoom();
        Assert.IsType<ViewResult>(result);
    }



    [Fact]
    public void Error_ReturnsView()
    {
        var result = _controller.Error();
        Assert.IsType<ViewResult>(result);
    }
}