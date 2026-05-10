using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Planora.Domain.Entities;
using Planora.Domain.Constants;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;
using Planora.Web.Controllers;
using Xunit;

namespace Planora.Tests;

public class ScheduleControllerTests
{
    private readonly Mock<IScheduleService> _scheduleServiceMock;
    private readonly Mock<IGroupService> _groupServiceMock;
    private readonly Mock<IExportService> _exportServiceMock;
    private readonly Mock<IClassroomService> _classroomServiceMock;
    private readonly Mock<ISubjectService> _subjectServiceMock;
    private readonly Mock<ITimeSlotService> _timeSlotServiceMock;
    private readonly Mock<IScheduleNoteService> _scheduleNoteServiceMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly ScheduleController _controller;

    public ScheduleControllerTests()
    {
        _scheduleServiceMock = new Mock<IScheduleService>();
        _groupServiceMock = new Mock<IGroupService>();
        _exportServiceMock = new Mock<IExportService>();
        _classroomServiceMock = new Mock<IClassroomService>();
        _subjectServiceMock = new Mock<ISubjectService>();
        _timeSlotServiceMock = new Mock<ITimeSlotService>();
        _scheduleNoteServiceMock = new Mock<IScheduleNoteService>();

        var userStore = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStore.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);

        _controller = new ScheduleController(
            _scheduleServiceMock.Object,
            _groupServiceMock.Object,
            _exportServiceMock.Object,
            _classroomServiceMock.Object,
            _subjectServiceMock.Object,
            _timeSlotServiceMock.Object,
            _scheduleNoteServiceMock.Object,
            _userManagerMock.Object);
    }

    private void SetupGroupsAndTeachers()
    {
        _groupServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<GroupDto>
        {
            new GroupDto { Id = 1, Name = "G1" }
        });

        var users = new List<User>
        {
            new User { Id = "t1", FullName = "Teacher 1" }
        }.AsQueryable();

        _userManagerMock.Setup(u => u.Users).Returns(users);
        _userManagerMock.Setup(u => u.GetUsersInRoleAsync(AppRoles.Teacher)).ReturnsAsync(users.ToList());
        _subjectServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<SubjectDto>());
    }

    [Fact]
    public async Task Index_WhenGroupIdProvided_ReturnsGroupSchedule()
    {
        SetupGroupsAndTeachers();

        _scheduleServiceMock.Setup(s => s.GetByGroupIdAsync(1)).ReturnsAsync(new List<ScheduleEntryDto>
        {
            new ScheduleEntryDto { Id = 1 }
        });

        var result = await _controller.Index(1, null);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<ScheduleEntryDto>>(viewResult.Model);
        Assert.Single(model);
        Assert.IsType<SelectList>(_controller.ViewBag.Groups);
        Assert.IsType<SelectList>(_controller.ViewBag.Teachers);
    }

    [Fact]
    public async Task Index_WhenTeacherIdProvided_ReturnsTeacherSchedule()
    {
        SetupGroupsAndTeachers();

        _scheduleServiceMock.Setup(s => s.GetByTeacherIdAsync("t1")).ReturnsAsync(new List<ScheduleEntryDto>
        {
            new ScheduleEntryDto { Id = 2 }
        });

        var result = await _controller.Index(null, "t1");

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<ScheduleEntryDto>>(viewResult.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task Index_WhenNoFilters_ReturnsAllSchedule()
    {
        SetupGroupsAndTeachers();

        _scheduleServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ScheduleEntryDto>
        {
            new ScheduleEntryDto { Id = 3 }
        });

        var result = await _controller.Index(null, null);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<ScheduleEntryDto>>(viewResult.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task Today_WhenGroupIdProvided_ReturnsTodayGroupSchedule()
    {
        SetupGroupsAndTeachers();

        _scheduleServiceMock.Setup(s => s.GetTodayByGroupIdAsync(1)).ReturnsAsync(new List<ScheduleEntryDto>
        {
            new ScheduleEntryDto { Id = 4 }
        });

        var result = await _controller.Today(1, null);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<ScheduleEntryDto>>(viewResult.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task Today_WhenTeacherIdProvided_ReturnsTodayTeacherSchedule()
    {
        SetupGroupsAndTeachers();

        _scheduleServiceMock.Setup(s => s.GetTodayByTeacherIdAsync("t1")).ReturnsAsync(new List<ScheduleEntryDto>
        {
            new ScheduleEntryDto { Id = 5 }
        });

        var result = await _controller.Today(null, "t1");

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<ScheduleEntryDto>>(viewResult.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task Today_WhenNoFilters_ReturnsEmptyEnumerable()
    {
        SetupGroupsAndTeachers();

        var result = await _controller.Today(null, null);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<ScheduleEntryDto>>(viewResult.Model);
        Assert.Empty(model);
    }

    [Fact]
    public async Task TeacherLocation_WhenTeacherIdProvided_ReturnsLocation()
    {
        var users = new List<User>
        {
            new User { Id = "t1", FullName = "Teacher 1" }
        }.AsQueryable();

        _userManagerMock.Setup(u => u.Users).Returns(users);
        _userManagerMock.Setup(u => u.GetUsersInRoleAsync(AppRoles.Teacher)).ReturnsAsync(users.ToList());

        _scheduleServiceMock.Setup(s => s.FindTeacherLocationAsync("t1")).ReturnsAsync(new TeacherLocationDto
        {
            TeacherId = "t1",
            TeacherName = "Teacher 1",
            IsTeachingNow = true
        });

        var result = await _controller.TeacherLocation("t1");

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<TeacherLocationDto>(viewResult.Model);
        Assert.Equal("t1", model.TeacherId);
        Assert.IsType<SelectList>(_controller.ViewBag.Teachers);
    }

    [Fact]
    public async Task TeacherLocation_WhenTeacherIdMissing_ReturnsEmptyView()
    {
        var users = new List<User>
        {
            new User { Id = "t1", FullName = "Teacher 1" }
        }.AsQueryable();

        _userManagerMock.Setup(u => u.Users).Returns(users);
        _userManagerMock.Setup(u => u.GetUsersInRoleAsync(AppRoles.Teacher)).ReturnsAsync(users.ToList());

        var result = await _controller.TeacherLocation(null);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.Model);
    }

    [Fact]
    public async Task SearchTeachers_WhenQueryProvided_ReturnsResults()
    {
        _scheduleServiceMock.Setup(s => s.SearchTeachersAsync("ivan")).ReturnsAsync(new List<TeacherSearchDto>
        {
            new TeacherSearchDto { Id = "1", FullName = "Ivan Ivanov" }
        });

        var result = await _controller.SearchTeachers("ivan");

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<TeacherSearchDto>>(viewResult.Model);
        Assert.Single(model);
        Assert.Equal("ivan", _controller.ViewBag.Query);
    }

    [Fact]
    public async Task SearchTeachers_WhenQueryMissing_ReturnsEmptyEnumerable()
    {
        var result = await _controller.SearchTeachers(null);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<TeacherSearchDto>>(viewResult.Model);
        Assert.Empty(model);
    }

    [Fact]
    public async Task DeleteEntry_Post_RedirectsToIndex()
    {
        var result = await _controller.DeleteEntry(1, null, null);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _scheduleServiceMock.Verify(s => s.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task ClearSchedule_Post_RedirectsToIndex()
    {
        var result = await _controller.ClearSchedule();

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _scheduleServiceMock.Verify(s => s.DeleteAllAsync(), Times.Once);
    }

    #region UpdateOnlineStatus Tests

    [Fact]
    public async Task UpdateOnlineStatus_WhenUserNotAuthenticated_ReturnsUnauthorized()
    {
        _userManagerMock.Setup(u => u.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns((string)null);
        var dto = new UpdateScheduleOnlineStatusDto { IsOnline = true, MeetingLink = "https://meet.google.com/test" };

        var result = await _controller.UpdateOnlineStatus(1, dto);

        Assert.IsType<UnauthorizedResult>(result);
        _scheduleServiceMock.Verify(s => s.UpdateOnlineStatusAsync(It.IsAny<int>(), It.IsAny<UpdateScheduleOnlineStatusDto>()), Times.Never);
    }

    [Fact]
    public async Task UpdateOnlineStatus_WhenScheduleNotFound_ReturnsNotFound()
    {
        var userId = "teacher1";
        _userManagerMock.Setup(u => u.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns(userId);
        _scheduleServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((ScheduleEntryDto)null);
        var dto = new UpdateScheduleOnlineStatusDto { IsOnline = true, MeetingLink = "https://meet.google.com/test" };

        var result = await _controller.UpdateOnlineStatus(999, dto);

        Assert.IsType<NotFoundResult>(result);
        _scheduleServiceMock.Verify(s => s.UpdateOnlineStatusAsync(It.IsAny<int>(), It.IsAny<UpdateScheduleOnlineStatusDto>()), Times.Never);
    }

    [Fact]
    public async Task UpdateOnlineStatus_WhenUserIsNotOwner_ReturnsForbid()
    {
        var userId = "teacher1";
        var otherTeacherId = "teacher2";
        _userManagerMock.Setup(u => u.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns(userId);
        
        _scheduleServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new ScheduleEntryDto { TeacherId = otherTeacherId });
        var dto = new UpdateScheduleOnlineStatusDto { IsOnline = true, MeetingLink = "https://meet.google.com/test" };

        var result = await _controller.UpdateOnlineStatus(1, dto);

        Assert.IsType<ForbidResult>(result);
        _scheduleServiceMock.Verify(s => s.UpdateOnlineStatusAsync(It.IsAny<int>(), It.IsAny<UpdateScheduleOnlineStatusDto>()), Times.Never);
    }

    [Fact]
    public async Task UpdateOnlineStatus_WhenUserIsOwner_UpdatesStatusAndReturnsOk()
    {
        var userId = "teacher_owner";
        _userManagerMock.Setup(u => u.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns(userId);
        
        _scheduleServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new ScheduleEntryDto { TeacherId = userId });
        var dto = new UpdateScheduleOnlineStatusDto { IsOnline = true, MeetingLink = "https://meet.google.com/test" };

        var result = await _controller.UpdateOnlineStatus(1, dto);

        Assert.IsType<OkResult>(result);
        _scheduleServiceMock.Verify(s => s.UpdateOnlineStatusAsync(1, dto), Times.Once);
    }

    #endregion
}