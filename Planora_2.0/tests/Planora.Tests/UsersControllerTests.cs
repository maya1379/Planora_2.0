using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Planora.Domain.Entities;
using Planora.Domain.Enums;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;
using Planora.Web.Controllers;
using Planora.Web.ViewModels;
using Xunit;

namespace Planora.Tests;

public class UsersControllerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IGroupService> _groupServiceMock;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        var userStore = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStore.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);

        _groupServiceMock = new Mock<IGroupService>();

        _controller = new UsersController(_userManagerMock.Object, _groupServiceMock.Object);
    }

    [Fact]
    public void Index_ReturnsViewWithOrderedUsers()
    {
        var users = new List<User>
        {
            new User { Id = "2", FullName = "Petrenko Ivan" },
            new User { Id = "1", FullName = "Andrienko Oleg" }
        }.AsQueryable();

        _userManagerMock.Setup(u => u.Users).Returns(users);

        var result = _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<User>>(viewResult.Model).ToList();

        Assert.Equal(2, model.Count);
       // Assert.Equal("Andrienko Oleg", model[0].FullName);
        Assert.Equal("Andrienko leg", model[0].FullName);
        Assert.Equal("Petrenko Ivan", model[1].FullName);
    }

    [Fact]
    public async Task Create_Get_ReturnsViewWithDefaultStudentRoleAndGroups()
    {
        _groupServiceMock.Setup(g => g.GetAllAsync()).ReturnsAsync(new List<GroupDto>
        {
            new GroupDto { Id = 1, Name = "G1" }
        });

        var result = await _controller.Create();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CreateUserViewModel>(viewResult.Model);

        Assert.Equal(UserRole.Student, model.Role);
        Assert.IsType<SelectList>(_controller.ViewBag.Groups);
    }

    [Fact]
    public async Task Create_Post_WhenModelStateInvalid_ReturnsViewWithGroups()
    {
        _controller.ModelState.AddModelError("Email", "Required");

        _groupServiceMock.Setup(g => g.GetAllAsync()).ReturnsAsync(new List<GroupDto>
        {
            new GroupDto { Id = 1, Name = "G1" }
        });

        var model = new CreateUserViewModel
        {
            FullName = "User",
            Email = "",
            Password = "123456",
            Role = UserRole.Student
        };

        var result = await _controller.Create(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(model, viewResult.Model);
        Assert.IsType<SelectList>(_controller.ViewBag.Groups);
    }

    [Fact]
    public async Task Create_Post_WhenSucceeded_RedirectsToIndex()
    {
        _userManagerMock
            .Setup(u => u.CreateAsync(It.IsAny<User>(), "123456"))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(u => u.AddToRoleAsync(It.IsAny<User>(), "Teacher"))
            .ReturnsAsync(IdentityResult.Success);

        var model = new CreateUserViewModel
        {
            FullName = "Teacher User",
            Email = "teacher@test.com",
            Password = "123456",
            Role = UserRole.Teacher,
            Faculty = "FIT",
            Position = "Assistant",
            GroupId = 99
        };

        var result = await _controller.Create(model);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);

        _userManagerMock.Verify(u => u.CreateAsync(
            It.Is<User>(x =>
                x.UserName == model.Email &&
                x.Email == model.Email &&
                x.FullName == model.FullName &&
                x.Role == model.Role &&
                x.Faculty == model.Faculty &&
                x.Position == model.Position &&
                x.GroupId == null &&
                x.EmailConfirmed),
            model.Password), Times.Once);

        _userManagerMock.Verify(u => u.AddToRoleAsync(It.IsAny<User>(), "Teacher"), Times.Once);
    }

    [Fact]
    public async Task Create_Post_WhenCreateFails_ReturnsViewWithErrors()
    {
        var failed = IdentityResult.Failed(new IdentityError { Description = "Create failed" });

        _userManagerMock
            .Setup(u => u.CreateAsync(It.IsAny<User>(), "123456"))
            .ReturnsAsync(failed);

        var model = new CreateUserViewModel
        {
            FullName = "Student User",
            Email = "student@test.com",
            Password = "123456",
            Role = UserRole.Student,
            GroupId = 1
        };

        var result = await _controller.Create(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(model, viewResult.Model);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Edit_Get_WhenUserExists_ReturnsViewWithModelAndGroups()
    {
        var user = new User
        {
            Id = "1",
            FullName = "User One",
            Faculty = "FIT",
            Position = "Teacher",
            GroupId = 2,
            Role = UserRole.Teacher
        };

        _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(user);
        _groupServiceMock.Setup(g => g.GetAllAsync()).ReturnsAsync(new List<GroupDto>
        {
            new GroupDto { Id = 1, Name = "G1" }
        });

        var result = await _controller.Edit("1");

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EditUserViewModel>(viewResult.Model);

        Assert.Equal("1", model.Id);
        Assert.Equal("User One", model.FullName);
        Assert.IsType<SelectList>(_controller.ViewBag.Groups);
    }

    [Fact]
    public async Task Edit_Get_WhenUserMissing_ReturnsNotFound()
    {
        _userManagerMock.Setup(u => u.FindByIdAsync("99")).ReturnsAsync((User?)null);

        var result = await _controller.Edit("99");

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_WhenModelStateInvalid_ReturnsViewWithGroups()
    {
        _controller.ModelState.AddModelError("FullName", "Required");

        _groupServiceMock.Setup(g => g.GetAllAsync()).ReturnsAsync(new List<GroupDto>
        {
            new GroupDto { Id = 1, Name = "G1" }
        });

        var model = new EditUserViewModel
        {
            Id = "1",
            FullName = ""
        };

        var result = await _controller.Edit(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(model, viewResult.Model);
        Assert.IsType<SelectList>(_controller.ViewBag.Groups);
    }

    [Fact]
    public async Task Edit_Post_WhenUserMissing_ReturnsNotFound()
    {
        _userManagerMock.Setup(u => u.FindByIdAsync("99")).ReturnsAsync((User?)null);

        var model = new EditUserViewModel
        {
            Id = "99",
            FullName = "Unknown"
        };

        var result = await _controller.Edit(model);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_WhenSucceeded_UpdatesUserAndRedirects()
    {
        var user = new User
        {
            Id = "1",
            FullName = "Old Name",
            Faculty = "Old Faculty",
            Position = "Old Position",
            GroupId = 5,
            Role = UserRole.Student
        };

        _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        var model = new EditUserViewModel
        {
            Id = "1",
            FullName = "New Name",
            Faculty = "New Faculty",
            Position = "New Position",
            GroupId = 3
        };

        var result = await _controller.Edit(model);

        Assert.Equal("New Name", user.FullName);
        Assert.Equal("New Faculty", user.Faculty);
        Assert.Equal("New Position", user.Position);
        Assert.Equal(3, user.GroupId);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);

        _userManagerMock.Verify(u => u.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task Edit_Post_WhenUpdateFails_ReturnsViewWithErrors()
    {
        var user = new User
        {
            Id = "1",
            FullName = "Old Name",
            Role = UserRole.Teacher
        };

        _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.UpdateAsync(user)).ReturnsAsync(
            IdentityResult.Failed(new IdentityError { Description = "Update failed" }));

        var model = new EditUserViewModel
        {
            Id = "1",
            FullName = "New Name"
        };

        var result = await _controller.Edit(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(model, viewResult.Model);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Delete_WhenUserExists_DeletesAndRedirects()
    {
        var user = new User { Id = "1", FullName = "User One" };

        _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

        var result = await _controller.Delete("1");

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _userManagerMock.Verify(u => u.DeleteAsync(user), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenUserMissing_StillRedirects()
    {
        _userManagerMock.Setup(u => u.FindByIdAsync("99")).ReturnsAsync((User?)null);

        var result = await _controller.Delete("99");

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _userManagerMock.Verify(u => u.DeleteAsync(It.IsAny<User>()), Times.Never);
    }
}