using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Planora.Domain.Entities;
using Planora.Domain.Constants;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;
using Planora.Tests.Helpers;
using Planora.Web.Controllers;
using Xunit;

namespace Planora.Tests;

public class TeachingAssignmentsControllerTests
{
    private readonly Mock<ITeachingAssignmentService> _assignmentServiceMock;
    private readonly Mock<ISubjectService> _subjectServiceMock;
    private readonly Mock<IGroupService> _groupServiceMock;
    private readonly Mock<UserManager<User>> _userManagerMock;

    private readonly TeachingAssignmentsController _controller;

    public TeachingAssignmentsControllerTests()
    {
        _assignmentServiceMock = new Mock<ITeachingAssignmentService>();
        _subjectServiceMock = new Mock<ISubjectService>();
        _groupServiceMock = new Mock<IGroupService>();

        var userStore = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStore.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);

        _controller = new TeachingAssignmentsController(
            _assignmentServiceMock.Object,
            _subjectServiceMock.Object,
            _groupServiceMock.Object,
            _userManagerMock.Object);
    }

    private void SetupDropdowns()
    {
        var teachers = new List<User>
        {
            new User { Id = "t1", FullName = "Teacher 1" },
            new User { Id = "t2", FullName = "Teacher 2" },
            new User { Id = "s1", FullName = "Student 1" }
        };

        var asyncQueryableTeachers = new TestAsyncEnumerable<User>(teachers);

        _userManagerMock
            .Setup(u => u.Users)
            .Returns(asyncQueryableTeachers);

        _userManagerMock.Setup(u => u.GetUsersInRoleAsync(AppRoles.Teacher)).ReturnsAsync(teachers);

        _subjectServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<SubjectDto>
        {
            new SubjectDto { Id = 1, Name = "Math" }
        });

        _groupServiceMock.Setup(g => g.GetAllAsync()).ReturnsAsync(new List<GroupDto>
        {
            new GroupDto { Id = 1, Name = "G1" }
        });
    }

    [Fact]
    public async Task Index_ReturnsViewWithAssignments()
    {
        var data = new List<TeachingAssignmentDto>
        {
            new TeachingAssignmentDto { Id = 1 }
        };

        _assignmentServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(data);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<TeachingAssignmentDto>>(viewResult.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task Create_Get_ReturnsViewAndPopulatesDropdowns()
    {
        SetupDropdowns();

        var result = await _controller.Create();

        var viewResult = Assert.IsType<ViewResult>(result);

        Assert.IsType<SelectList>(_controller.ViewBag.Teachers);
        Assert.IsType<SelectList>(_controller.ViewBag.Subjects);
        Assert.IsType<SelectList>(_controller.ViewBag.Groups);
        Assert.Null(viewResult.ViewName);
    }

    [Fact]
    public async Task Create_Post_WhenValid_RedirectsToIndex()
    {
        var dto = new CreateTeachingAssignmentDto();

        var result = await _controller.Create(dto);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        _assignmentServiceMock.Verify(s => s.CreateAsync(dto), Times.Once);
    }

    [Fact]
    public async Task Create_Post_WhenInvalid_ReturnsViewWithDropdowns()
    {
        SetupDropdowns();
        _controller.ModelState.AddModelError("Error", "Invalid");

        var dto = new CreateTeachingAssignmentDto();

        var result = await _controller.Create(dto);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(dto, viewResult.Model);

        Assert.NotNull(_controller.ViewBag.Teachers);
        Assert.NotNull(_controller.ViewBag.Subjects);
        Assert.NotNull(_controller.ViewBag.Groups);
    }

    [Fact]
    public async Task Edit_Get_WhenExists_ReturnsViewWithDto()
    {
        SetupDropdowns();

        _assignmentServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(
            new TeachingAssignmentDto
            {
                Id = 1,
                TeacherId = "t1",
                SubjectId = 1,
                GroupId = 1,
                HoursPerWeek = 2
            });

        var result = await _controller.Edit(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<UpdateTeachingAssignmentDto>(viewResult.Model);

        Assert.Equal(1, model.Id);
        Assert.Equal("t1", model.TeacherId);
        Assert.Equal(1, model.SubjectId);
        Assert.Equal(1, model.GroupId);
        Assert.Equal(2, model.HoursPerWeek);

        Assert.NotNull(_controller.ViewBag.Teachers);
        Assert.NotNull(_controller.ViewBag.Subjects);
        Assert.NotNull(_controller.ViewBag.Groups);
    }

    [Fact]
    public async Task Edit_Get_WhenNotFound_ReturnsNotFound()
    {
        _assignmentServiceMock.Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync((TeachingAssignmentDto?)null);

        var result = await _controller.Edit(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_WhenValid_RedirectsToIndex()
    {
        var dto = new UpdateTeachingAssignmentDto { Id = 1 };

        var result = await _controller.Edit(dto);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        _assignmentServiceMock.Verify(s => s.UpdateAsync(dto), Times.Once);
    }

    [Fact]
    public async Task Edit_Post_WhenInvalid_ReturnsViewWithDropdowns()
    {
        SetupDropdowns();
        _controller.ModelState.AddModelError("Error", "Invalid");

        var dto = new UpdateTeachingAssignmentDto();

        var result = await _controller.Edit(dto);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(dto, viewResult.Model);

        Assert.NotNull(_controller.ViewBag.Teachers);
        Assert.NotNull(_controller.ViewBag.Subjects);
        Assert.NotNull(_controller.ViewBag.Groups);
    }

    [Fact]
    public async Task Delete_RedirectsToIndex()
    {
        var result = await _controller.Delete(1);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        _assignmentServiceMock.Verify(s => s.DeleteAsync(1), Times.Once);
    }
}