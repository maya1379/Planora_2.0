using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;
using Planora.Web.Controllers;
using Xunit;

namespace Planora.Tests;

public class ClassroomsControllerTests
{
    private readonly Mock<IClassroomService> _classroomServiceMock;
    private readonly Mock<IBuildingService> _buildingServiceMock;
    private readonly Mock<IScheduleService> _scheduleServiceMock;
    private readonly ClassroomsController _controller;

    public ClassroomsControllerTests()
    {
        _classroomServiceMock = new Mock<IClassroomService>();
        _buildingServiceMock = new Mock<IBuildingService>();
        _scheduleServiceMock = new Mock<IScheduleService>();

<<<<<<< HEAD
        _controller = new ClassroomsController(
            _classroomServiceMock.Object, 
            _buildingServiceMock.Object, 
            _scheduleServiceMock.Object);
=======
        _controller = new ClassroomsController(_classroomServiceMock.Object, _buildingServiceMock.Object, _scheduleServiceMock.Object);
>>>>>>> Artur-17
        _controller.TempData = new Mock<ITempDataDictionary>().Object;
    }

    [Fact]
    public async Task Index_WithoutFilter_ReturnsAllClassrooms()
    {
        var classrooms = new List<ClassroomDto>
        {
            new ClassroomDto { Id = 1, BuildingId = 1, Number = "101" },
            new ClassroomDto { Id = 2, BuildingId = 2, Number = "102" }
        };

        _classroomServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(classrooms);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<ClassroomDto>>(viewResult.Model);
        Assert.Equal(2, model.Count());
    }

    [Fact]
    public async Task Index_WithFilter_ReturnsFilteredClassrooms()
    {
        var classrooms = new List<ClassroomDto>
        {
            new ClassroomDto { Id = 1, BuildingId = 1, Number = "101" },
            new ClassroomDto { Id = 2, BuildingId = 2, Number = "102" }
        };

        _classroomServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(classrooms);

        var result = await _controller.Index(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<ClassroomDto>>(viewResult.Model);
        Assert.Single(model);
        Assert.Equal(1, model.First().BuildingId);
    }

    [Fact]
    public async Task AdminIndex_WithFilter_ReturnsFilteredClassrooms()
    {
        var classrooms = new List<ClassroomDto>
        {
            new ClassroomDto { Id = 1, BuildingId = 1, Number = "101" },
            new ClassroomDto { Id = 2, BuildingId = 2, Number = "102" }
        };

        _classroomServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(classrooms);

        var result = await _controller.AdminIndex(2);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<ClassroomDto>>(viewResult.Model);
        Assert.Single(model);
        Assert.Equal(2, model.First().BuildingId);
    }

    [Fact]
    public async Task Create_Get_ReturnsViewAndPopulatesBuildings()
    {
        _buildingServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<BuildingDto>
        {
            new BuildingDto { Id = 1, Name = "B1" }
        });

        var result = await _controller.Create();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(_controller.ViewBag.Buildings);
        Assert.IsType<SelectList>(_controller.ViewBag.Buildings);
        Assert.Null(viewResult.ViewName);
    }

    [Fact]
    public async Task Create_Post_WhenModelStateInvalid_ReturnsViewWithDto()
    {
        _controller.ModelState.AddModelError("Number", "Required");

        _buildingServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<BuildingDto>
        {
            new BuildingDto { Id = 1, Name = "B1" }
        });

        var dto = new CreateClassroomDto();

        var result = await _controller.Create(dto);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(dto, viewResult.Model);
        Assert.NotNull(_controller.ViewBag.Buildings);
    }

    [Fact]
    public async Task Create_Post_WhenValid_RedirectsToAdminIndex()
    {
        var dto = new CreateClassroomDto();

        var result = await _controller.Create(dto);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("AdminIndex", redirectResult.ActionName);
        _classroomServiceMock.Verify(s => s.CreateAsync(dto), Times.Once);
    }

    [Fact]
    public async Task Edit_Get_WhenClassroomExists_ReturnsViewWithUpdateDto()
    {
        _classroomServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new ClassroomDto
        {
            Id = 1,
            Number = "101",
            Capacity = 30,
            HasComputers = true,
            HasProjector = true,
            Faculty = "FIT",
            BuildingId = 2
        });

        _buildingServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<BuildingDto>
        {
            new BuildingDto { Id = 2, Name = "B2" }
        });

        var result = await _controller.Edit(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<UpdateClassroomDto>(viewResult.Model);

        Assert.Equal(1, model.Id);
        Assert.Equal("101", model.Number);
        Assert.Equal(2, model.BuildingId);
        Assert.NotNull(_controller.ViewBag.Buildings);
    }

    [Fact]
    public async Task Edit_Get_WhenClassroomMissing_ReturnsNotFound()
    {
        _classroomServiceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((ClassroomDto?)null);

        var result = await _controller.Edit(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_WhenModelStateInvalid_ReturnsViewWithDto()
    {
        _controller.ModelState.AddModelError("Number", "Required");

        _buildingServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<BuildingDto>
        {
            new BuildingDto { Id = 1, Name = "B1" }
        });

        var dto = new UpdateClassroomDto { Id = 1, BuildingId = 1 };

        var result = await _controller.Edit(dto);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(dto, viewResult.Model);
        Assert.NotNull(_controller.ViewBag.Buildings);
    }

    [Fact]
    public async Task Edit_Post_WhenValid_RedirectsToAdminIndex()
    {
        var dto = new UpdateClassroomDto { Id = 1, BuildingId = 1 };

        var result = await _controller.Edit(dto);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("AdminIndex", redirectResult.ActionName);
        _classroomServiceMock.Verify(s => s.UpdateAsync(dto), Times.Once);
    }

    [Fact]
    public async Task Delete_Post_RedirectsToAdminIndex()
    {
        var result = await _controller.Delete(1);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("AdminIndex", redirectResult.ActionName);
        _classroomServiceMock.Verify(s => s.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task FreeRooms_ReturnsViewWithFreeRooms()
    {
        var freeRooms = new List<ClassroomDto>
        {
            new ClassroomDto { Id = 1, Number = "201" }
        };

        _classroomServiceMock.Setup(s => s.FindFreeClassroomsNowAsync()).ReturnsAsync(freeRooms);

        var result = await _controller.FreeRooms();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<ClassroomDto>>(viewResult.Model);
        Assert.Single(model);
    }
}