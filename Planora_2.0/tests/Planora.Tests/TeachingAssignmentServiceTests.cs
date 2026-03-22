using AutoMapper;
using Moq;
using Planora.Domain.Entities;
using Planora.Services.DTOs;
using Planora.Services.Interfaces;
using Planora.Services.Mapping;
using Planora.Services.Services;
using Xunit;

namespace Planora.Tests;

public class TeachingAssignmentServiceTests
{
    private readonly Mock<ITeachingAssignmentRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly TeachingAssignmentService _service;

    public TeachingAssignmentServiceTests()
    {
        _repositoryMock = new Mock<ITeachingAssignmentRepository>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        _mapper = config.CreateMapper();
        _service = new TeachingAssignmentService(_repositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAllAsync_WhenAssignmentsExist_ReturnsDtos()
    {
        var data = new List<TeachingAssignment>
        {
            new TeachingAssignment
            {
                Id = 1,
                Teacher = new User { FullName = "Teacher 1" },
                Subjects = new Subjects { Name = "Math" }
            }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(data);

        var result = (await _service.GetAllAsync()).ToList();

        Assert.Single(result);
        Assert.Equal("Teacher 1", result[0].TeacherName);
        Assert.Equal("Math", result[0].SubjectName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsDto()
    {
        var entity = new TeachingAssignment
        {
            Id = 2,
            Teacher = new User { FullName = "Teacher 2" },
            Subjects = new Subjects { Name = "Physics" }
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(entity);

        var result = await _service.GetByIdAsync(2);

        Assert.NotNull(result);
        Assert.Equal("Teacher 2", result!.TeacherName);
        Assert.Equal("Physics", result.SubjectName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((TeachingAssignment?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByTeacherIdAsync_WhenCalled_ReturnsDtos()
    {
        var data = new List<TeachingAssignment>
        {
            new TeachingAssignment
            {
                Id = 3,
                TeacherId = "teacher1",
                Teacher = new User { FullName = "Teacher A" }
            }
        };

        _repositoryMock.Setup(r => r.GetByTeacherIdAsync("teacher1"))
            .ReturnsAsync(data);

        var result = (await _service.GetByTeacherIdAsync("teacher1")).ToList();

        Assert.Single(result);
        Assert.Equal("Teacher A", result[0].TeacherName);
    }

    [Fact]
    public async Task CreateAsync_WhenValidDto_ReturnsCreatedDto()
    {
        var dto = new CreateTeachingAssignmentDto
        {
            TeacherId = "teacher1",
            SubjectId = 1,
            GroupId = 1
        };

        var created = new TeachingAssignment
        {
            Id = 10,
            Teacher = new User { FullName = "Created Teacher" },
            Subjects = new Subjects { Name = "Biology" }
        };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TeachingAssignment>()))
            .ReturnsAsync(created);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("Created Teacher", result.TeacherName);
        Assert.Equal("Biology", result.SubjectName);
    }

    [Fact]
    public async Task UpdateAsync_WhenExists_CallsRepository()
    {
        var existing = new TeachingAssignment { Id = 5 };

        var dto = new UpdateTeachingAssignmentDto
        {
            Id = 5,
            TeacherId = "teacher2"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(existing);

        await _service.UpdateAsync(dto);

        _repositoryMock.Verify(r => r.UpdateAsync(existing), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotExists_ThrowsException()
    {
        var dto = new UpdateTeachingAssignmentDto { Id = 100 };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(100))
            .ReturnsAsync((TeachingAssignment?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(dto));
    }

    [Fact]
    public async Task DeleteAsync_WhenCalled_CallsRepository()
    {
        int id = 7;

        await _service.DeleteAsync(id);

        _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
    }
}