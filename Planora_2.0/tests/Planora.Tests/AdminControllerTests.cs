using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Planora.Domain.Entities;
using Planora.Domain.Constants;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;
using Planora.Web.Controllers;
using Planora.Web.ViewModels;
using Xunit;

namespace Planora.Tests;

public class AdminControllerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IBuildingService> _buildingServiceMock;
    private readonly Mock<IGroupService> _groupServiceMock;
    private readonly Mock<IScheduleService> _scheduleServiceMock;
    private readonly Mock<ISubjectService> _subjectServiceMock;
    private readonly AdminController _controller;

    public AdminControllerTests()
    {
        var userStore = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStore.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);

        _buildingServiceMock = new Mock<IBuildingService>();
        _groupServiceMock = new Mock<IGroupService>();
        _scheduleServiceMock = new Mock<IScheduleService>();
        _subjectServiceMock = new Mock<ISubjectService>();

        _controller = new AdminController(
            _userManagerMock.Object,
            _buildingServiceMock.Object,
            _groupServiceMock.Object,
            _scheduleServiceMock.Object,
            _subjectServiceMock.Object);
    }

    [Fact]
    public async Task Index_ReturnsDashboardViewModelWithCounts()
    {
        var users = new List<User>
        {
            new User { Id = "1", FullName = "Student 1" },
            new User { Id = "2", FullName = "Student 2" },
            new User { Id = "3", FullName = "Teacher 1" },
            new User { Id = "4", FullName = "Admin 1" }
        }.AsQueryable();

        _userManagerMock.Setup(x => x.Users).Returns(users);

        var students = new List<User> { users.ElementAt(0), users.ElementAt(1) };
        var teachers = new List<User> { users.ElementAt(2) };

        _userManagerMock.Setup(x => x.GetUsersInRoleAsync(AppRoles.Student)).ReturnsAsync(students);
        _userManagerMock.Setup(x => x.GetUsersInRoleAsync(AppRoles.Teacher)).ReturnsAsync(teachers);

        _groupServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<GroupDto>
        {
            new GroupDto { Id = 1, Name = "G1" },
            new GroupDto { Id = 2, Name = "G2" }
        });

        _buildingServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<BuildingDto>
        {
            new BuildingDto { Id = 1, Name = "B1" },
            new BuildingDto { Id = 2, Name = "B2" },
            new BuildingDto { Id = 3, Name = "B3" }
        });

        _scheduleServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ScheduleEntryDto>
        {
            new ScheduleEntryDto { Id = 1 },
            new ScheduleEntryDto { Id = 2 },
            new ScheduleEntryDto { Id = 3 },
            new ScheduleEntryDto { Id = 4 }
        });

        _subjectServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<SubjectDto>
        {
            new SubjectDto { Id = 1 }
        });

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AdminDashboardViewModel>(viewResult.Model);

        Assert.Equal(2, model.TotalStudents);
        Assert.Equal(1, model.TotalTeachers);
        Assert.Equal(2, model.TotalGroups);
        Assert.Equal(3, model.TotalBuildings);
        Assert.Equal(4, model.TotalScheduleEntries);
        Assert.Equal(1, model.TotalSubjects);
        Assert.True(model.RecentUsers.Count <= 5);
    }
}