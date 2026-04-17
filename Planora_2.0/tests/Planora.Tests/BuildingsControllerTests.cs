using Microsoft.AspNetCore.Mvc;
using Moq;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;
using Planora.Web.Controllers;
using Xunit;

namespace Planora.Tests;

public class BuildingsControllerTests
{
    private readonly Mock<IBuildingService> _buildingServiceMock;
    private readonly BuildingsController _controller;

    public BuildingsControllerTests()
    {
        _buildingServiceMock = new Mock<IBuildingService>();
        _controller = new BuildingsController(_buildingServiceMock.Object);
        _controller.TempData = new Mock<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionary>().Object;
    }

    [Fact]
    public async Task Index_ReturnsViewWithBuildings()
    {
        var buildings = new List<BuildingDto>
        {
            new BuildingDto { Id = 1, Name = "B1" },
            new BuildingDto { Id = 2, Name = "B2" }
        };

        _buildingServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(buildings);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<BuildingDto>>(viewResult.Model);
        Assert.Equal(2, model.Count());
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
        var dto = new CreateBuildingDto { Name = "" };

        var result = await _controller.Create(dto);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(dto, viewResult.Model);
    }

    [Fact]
    public async Task Create_Post_WhenValid_RedirectsToIndex()
    {
        var dto = new CreateBuildingDto { Name = "Main корпус" };

        var result = await _controller.Create(dto);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _buildingServiceMock.Verify(s => s.CreateAsync(dto), Times.Once);
    }

    [Fact]
    public async Task Edit_Get_WhenBuildingExists_ReturnsViewWithUpdateDto()
    {
        _buildingServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new BuildingDto
        {
            Id = 1,
            Name = "B1",
            Address = "Addr"
        });

        var result = await _controller.Edit(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<UpdateBuildingDto>(viewResult.Model);
        Assert.Equal(1, model.Id);
        Assert.Equal("B1", model.Name);
    }

    [Fact]
    public async Task Edit_Get_WhenBuildingMissing_ReturnsNotFound()
    {
        _buildingServiceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((BuildingDto?)null);

        var result = await _controller.Edit(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_WhenModelStateInvalid_ReturnsViewWithDto()
    {
        _controller.ModelState.AddModelError("Name", "Required");
        var dto = new UpdateBuildingDto { Id = 1, Name = "" };

        var result = await _controller.Edit(dto);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(dto, viewResult.Model);
    }

    [Fact]
    public async Task Edit_Post_WhenValid_RedirectsToIndex()
    {
        var dto = new UpdateBuildingDto { Id = 1, Name = "Updated" };

        var result = await _controller.Edit(dto);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _buildingServiceMock.Verify(s => s.UpdateAsync(dto), Times.Once);
    }

    [Fact]
    public async Task Delete_Post_RedirectsToIndex()
    {
        var result = await _controller.Delete(1);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _buildingServiceMock.Verify(s => s.DeleteAsync(1), Times.Once);
    }
}