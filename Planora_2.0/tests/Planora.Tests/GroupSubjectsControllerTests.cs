using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using Planora.Services.DTOs;
using Planora.Services.Services.Interfaces;
using Planora.Web.Controllers;
using Xunit;

namespace Planora.Tests;

public class GroupSubjectsControllerTests
{
    private readonly Mock<IGroupSubjectService> _groupSubjectServiceMock;
    private readonly Mock<IGroupService> _groupServiceMock;
    private readonly Mock<ISubjectService> _subjectServiceMock;
    private readonly GroupSubjectsController _controller;

    public GroupSubjectsControllerTests()
    {
        _groupSubjectServiceMock = new Mock<IGroupSubjectService>();
        _groupServiceMock = new Mock<IGroupService>();
        _subjectServiceMock = new Mock<ISubjectService>();

        _controller = new GroupSubjectsController(
            _groupSubjectServiceMock.Object,
            _groupServiceMock.Object,
            _subjectServiceMock.Object);
    }

    [Fact]
    public async Task Index_ReturnsViewWithItems()
    {
        var items = new List<GroupSubjectDto>
        {
            new GroupSubjectDto { Id = 1, GroupName = "G1", SubjectName = "Math" }
        };

        _groupSubjectServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(items);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<GroupSubjectDto>>(viewResult.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task Create_Get_ReturnsViewAndPopulatesDropdowns()
    {
        _groupServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<GroupDto>
        {
            new GroupDto { Id = 1, Name = "G1" }
        });

        _subjectServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<SubjectDto>
        {
            new SubjectDto { Id = 1, Name = "Math" }
        });

        var result = await _controller.Create();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<SelectList>(_controller.ViewBag.Groups);
        Assert.IsType<SelectList>(_controller.ViewBag.Subjects);
        Assert.Null(viewResult.ViewName);
    }

    [Fact]
    public async Task Create_Post_WhenModelStateInvalid_ReturnsViewWithDto()
    {
        _controller.ModelState.AddModelError("GroupId", "Required");

        _groupServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<GroupDto>());
        _subjectServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<SubjectDto>());

        var dto = new CreateGroupSubjectDto();

        var result = await _controller.Create(dto);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(dto, viewResult.Model);
        Assert.NotNull(_controller.ViewBag.Groups);
        Assert.NotNull(_controller.ViewBag.Subjects);
    }

    [Fact]
    public async Task Create_Post_WhenValid_RedirectsToIndex()
    {
        var dto = new CreateGroupSubjectDto();

        var result = await _controller.Create(dto);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _groupSubjectServiceMock.Verify(s => s.CreateAsync(dto), Times.Once);
    }

    [Fact]
    public async Task Delete_Post_RedirectsToIndex()
    {
        var result = await _controller.Delete(1);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _groupSubjectServiceMock.Verify(s => s.DeleteAsync(1), Times.Once);
    }
}