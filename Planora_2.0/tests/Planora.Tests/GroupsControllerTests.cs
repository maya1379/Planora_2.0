using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Planora.Domain.Entities;
using Planora.Domain.Enums;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;
using Planora.Web.Controllers;
using Xunit;

namespace Planora.Tests;

public class GroupsControllerTests
{
    private readonly Mock<IGroupService> _groupServiceMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly GroupsController _controller;

    public GroupsControllerTests()
    {
        _groupServiceMock = new Mock<IGroupService>();

        var userStore = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStore.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);

        _controller = new GroupsController(_groupServiceMock.Object, _userManagerMock.Object);
    }

    [Fact]
    public async Task Index_ReturnsViewWithGroups()
    {
        _groupServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<GroupDto>
        {
            new GroupDto { Id = 1, Name = "G1" }
        });

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<GroupDto>>(viewResult.Model);
        Assert.Single(model);
    }

    [Fact]
    public void Create_Get_ReturnsView()
    {
        var result = _controller.Create();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_Post_WhenModelStateInvalid_ReturnsViewWithDto()
    {
        _controller.ModelState.AddModelError("Name", "Required");
        var dto = new CreateGroupDto();

        var result = await _controller.Create(dto);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(dto, viewResult.Model);
    }

    [Fact]
    public async Task Create_Post_WhenValid_RedirectsToIndex()
    {
        var dto = new CreateGroupDto();

        var result = await _controller.Create(dto);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _groupServiceMock.Verify(s => s.CreateAsync(dto), Times.Once);
    }

    [Fact]
    public async Task Edit_Get_WhenGroupExists_ReturnsViewWithUpdateDto()
    {
        _groupServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new GroupDto
        {
            Id = 1,
            Name = "G1",
            Faculty = "FIT",
            StudentCount = 20
        });

        var result = await _controller.Edit(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<UpdateGroupDto>(viewResult.Model);
        Assert.Equal(1, model.Id);
        Assert.Equal("G1", model.Name);
    }

    [Fact]
    public async Task Edit_Get_WhenGroupMissing_ReturnsNotFound()
    {
        _groupServiceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((GroupDto?)null);

        var result = await _controller.Edit(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_WhenModelStateInvalid_ReturnsViewWithDto()
    {
        _controller.ModelState.AddModelError("Name", "Required");
        var dto = new UpdateGroupDto();

        var result = await _controller.Edit(dto);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(dto, viewResult.Model);
    }

    [Fact]
    public async Task Edit_Post_WhenValid_RedirectsToIndex()
    {
        var dto = new UpdateGroupDto { Id = 1 };

        var result = await _controller.Edit(dto);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _groupServiceMock.Verify(s => s.UpdateAsync(dto), Times.Once);
    }

    [Fact]
    public async Task Details_WhenGroupMissing_ReturnsNotFound()
    {
        _groupServiceMock.Setup(s => s.GetByIdAsync(5)).ReturnsAsync((GroupDto?)null);

        var result = await _controller.Details(5);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_WhenGroupExists_ReturnsViewAndStudents()
    {
        _groupServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new GroupDto
        {
            Id = 1,
            Name = "G1"
        });

        var users = new List<User>
        {
            new User { Id = "1", FullName = "A Student", Role = UserRole.Student, GroupId = 1 },
            new User { Id = "2", FullName = "B Student", Role = UserRole.Student, GroupId = 1 },
            new User { Id = "3", FullName = "Teacher", Role = UserRole.Teacher }
        }.AsQueryable();

        _userManagerMock.Setup(u => u.Users).Returns(users);

        var result = await _controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<GroupDto>(viewResult.Model);
        Assert.NotNull(_controller.ViewBag.Students);
    }

    [Fact]
    public async Task AddStudents_Get_WhenGroupMissing_ReturnsNotFound()
    {
        _groupServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((GroupDto?)null);

        var result = await _controller.AddStudents(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AddStudents_Get_WhenGroupExists_ReturnsViewWithAvailableStudents()
    {
        _groupServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new GroupDto
        {
            Id = 1,
            Name = "G1"
        });

        var users = new List<User>
        {
            new User { Id = "1", FullName = "Free Student", Role = UserRole.Student, GroupId = null },
            new User { Id = "2", FullName = "Busy Student", Role = UserRole.Student, GroupId = 2 }
        }.AsQueryable();

        _userManagerMock.Setup(u => u.Users).Returns(users);

        var result = await _controller.AddStudents(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<User>>(viewResult.Model);
        Assert.Single(model);
        Assert.Equal("G1", _controller.ViewBag.GroupName);
        Assert.Equal(1, _controller.ViewBag.GroupId);
    }

    [Fact]
    public async Task AddStudents_Post_WhenNoStudentsSelected_RedirectsToDetails()
    {
        var result = await _controller.AddStudents(1, new List<string>());

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectResult.ActionName);
        Assert.Equal(1, redirectResult.RouteValues!["id"]);
    }

    [Fact]
    public async Task AddStudents_Post_WhenStudentsSelected_UpdatesStudentsAndRedirects()
    {
        var student = new User { Id = "1", FullName = "Student", GroupId = null };

        _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(student);
        _userManagerMock.Setup(u => u.UpdateAsync(student)).ReturnsAsync(IdentityResult.Success);

        var result = await _controller.AddStudents(2, new List<string> { "1" });

        Assert.Equal(2, student.GroupId);
        _userManagerMock.Verify(u => u.UpdateAsync(student), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectResult.ActionName);
        Assert.Equal(2, redirectResult.RouteValues!["id"]);
    }

    [Fact]
    public async Task RemoveStudent_WhenStudentInGroup_UpdatesAndRedirects()
    {
        var student = new User { Id = "1", GroupId = 3 };

        _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(student);
        _userManagerMock.Setup(u => u.UpdateAsync(student)).ReturnsAsync(IdentityResult.Success);

        var result = await _controller.RemoveStudent(3, "1");

        Assert.Null(student.GroupId);
        _userManagerMock.Verify(u => u.UpdateAsync(student), Times.Once);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectResult.ActionName);
        Assert.Equal(3, redirectResult.RouteValues!["id"]);
    }

    [Fact]
    public async Task Delete_Post_RedirectsToIndex()
    {
        var result = await _controller.Delete(1);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _groupServiceMock.Verify(s => s.DeleteAsync(1), Times.Once);
    }
}