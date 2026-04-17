using Microsoft.AspNetCore.Mvc;
using Moq;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;
using Planora.Web.Controllers;
using Xunit;

namespace Planora.Tests;

public class SubjectsControllerTests
{
    private readonly Mock<ISubjectService> _subjectServiceMock;
    private readonly SubjectsController _controller;

    public SubjectsControllerTests()
    {
        _subjectServiceMock = new Mock<ISubjectService>();
        _controller = new SubjectsController(_subjectServiceMock.Object);
        _controller.TempData = new Mock<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionary>().Object;
    }

    [Fact]
    public async Task Index_ReturnsViewWithSubjects()
    {
        var subjects = new List<SubjectDto>
        {
            new SubjectDto { Id = 1, Name = "Math" },
            new SubjectDto { Id = 2, Name = "Physics" }
        };

        _subjectServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(subjects);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<SubjectDto>>(viewResult.Model);
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
        var dto = new CreateSubjectDto();

        var result = await _controller.Create(dto);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(dto, viewResult.Model);
    }

    [Fact]
    public async Task Create_Post_WhenValid_RedirectsToIndex()
    {
        var dto = new CreateSubjectDto
        {
            Name = "Biology"
        };

        var result = await _controller.Create(dto);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _subjectServiceMock.Verify(s => s.CreateAsync(dto), Times.Once);
    }

    [Fact]
    public async Task Edit_Get_WhenSubjectExists_ReturnsViewWithUpdateDto()
    {
        _subjectServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new SubjectDto
        {
            Id = 1,
            Name = "Programming",
            Type = 0,
            Requirements = "None"
        });

        var result = await _controller.Edit(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<UpdateSubjectDto>(viewResult.Model);

        Assert.Equal(1, model.Id);
        Assert.Equal("Programming", model.Name);
        Assert.Equal("None", model.Requirements);
    }

    [Fact]
    public async Task Edit_Get_WhenSubjectMissing_ReturnsNotFound()
    {
        _subjectServiceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((SubjectDto?)null);

        var result = await _controller.Edit(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_WhenModelStateInvalid_ReturnsViewWithDto()
    {
        _controller.ModelState.AddModelError("Name", "Required");
        var dto = new UpdateSubjectDto();

        var result = await _controller.Edit(dto);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(dto, viewResult.Model);
    }

    [Fact]
    public async Task Edit_Post_WhenValid_RedirectsToIndex()
    {
        var dto = new UpdateSubjectDto
        {
            Id = 1,
            Name = "Updated Subject"
        };

        var result = await _controller.Edit(dto);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _subjectServiceMock.Verify(s => s.UpdateAsync(dto), Times.Once);
    }

    [Fact]
    public async Task Delete_Post_RedirectsToIndex()
    {
        var result = await _controller.Delete(1);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _subjectServiceMock.Verify(s => s.DeleteAsync(1), Times.Once);
    }
}