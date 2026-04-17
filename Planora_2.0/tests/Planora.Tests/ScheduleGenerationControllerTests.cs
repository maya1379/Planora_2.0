using Microsoft.AspNetCore.Mvc;
using Moq;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;
using Planora.Web.Controllers;
using Xunit;

namespace Planora.Tests;

public class ScheduleGenerationControllerTests
{
    private readonly Mock<IScheduleGenerationService> _generationServiceMock;
    private readonly ScheduleGenerationController _controller;

    public ScheduleGenerationControllerTests()
    {
        _generationServiceMock = new Mock<IScheduleGenerationService>();
        _controller = new ScheduleGenerationController(_generationServiceMock.Object);
    }

    [Fact]
    public void Index_ReturnsView()
    {
        var result = _controller.Index();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Generate_Post_ReturnsResultViewWithModel()
    {
        var generationResult = new ScheduleGenerationResultDto
        {
            Success = true,
            TotalEntriesCreated = 10
        };

        _generationServiceMock
            .Setup(s => s.GenerateScheduleAsync())
            .ReturnsAsync(generationResult);

        var result = await _controller.Generate();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Result", viewResult.ViewName);
        Assert.Same(generationResult, viewResult.Model);
    }
}