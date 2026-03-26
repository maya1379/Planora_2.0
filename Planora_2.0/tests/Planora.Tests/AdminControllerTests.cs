using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Planora.Domain.Entities;
using Planora.Domain.Enums;
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

        _controller = new AdminController(
            _userManagerMock.Object,
            _buildingServiceMock.Object,
            _groupServiceMock.Object,
            _scheduleServiceMock.Object);
    }

    [Fact]
    public async Task Index_ReturnsDashboardViewModelWithCounts()
    {
        var users = new List<User>
        {
            new User { Id = "1", FullName = "Student 1", Role = UserRole.Student },
            new User { Id = "2", FullName = "Student 2", Role = UserRole.Student },
            new User { Id = "3", FullName = "Teacher 1", Role = UserRole.Teacher },
            new User { Id = "4", FullName = "Admin 1", Role = UserRole.Admin }
        }.AsQueryable();

        _userManagerMock.Setup(x => x.Users).Returns(users);

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

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AdminDashboardViewModel>(viewResult.Model);

        Assert.Equal(2, model.TotalStudents);
        Assert.Equal(1, model.TotalTeachers);
        Assert.Equal(2, model.TotalGroups);
        Assert.Equal(3, model.TotalBuildings);
        Assert.Equal(4, model.TotalScheduleEntries);
        Assert.True(model.RecentUsers.Count <= 5);
    }
}