using AutoMapper;
using Moq;
using Planora.Domain.Entities;
using Planora.Services.DTOs;
using Planora.Services.Interfaces;
using Planora.Services.Mapping;
using Planora.Services.Services;
using Xunit;

namespace Planora.Tests;

public class SubjectServiceTests
{
    private readonly Mock<ISubjectRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly SubjectService _service;

    public SubjectServiceTests()
    {
        _repositoryMock = new Mock<ISubjectRepository>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        _mapper = config.CreateMapper();
        _service = new SubjectService(_repositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAllAsync_WhenSubjectsExist_ReturnsMappedDtos()
    {
        var subjects = new List<Subjects>
        {
            new Subjects { Id = 1, Name = "Math" },
            new Subjects { Id = 2, Name = "Physics" }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(subjects);

        var result = (await _service.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("Math", result[0].Name);
        Assert.Equal("Physics", result[1].Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSubjectExists_ReturnsDto()
    {
        var subject = new Subjects
        {
            Id = 1,
            Name = "Programming"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(subject);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal("Programming", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSubjectDoesNotExist_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Subjects?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WhenDtoIsValid_ReturnsCreatedDto()
    {
        var dto = new CreateSubjectDto
        {
            Name = "Biology"
        };

        var created = new Subjects
        {
            Id = 10,
            Name = "Biology"
        };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Subjects>()))
            .ReturnsAsync(created);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("Biology", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenSubjectExists_CallsRepositoryUpdate()
    {
        var existing = new Subjects
        {
            Id = 5,
            Name = "Old Name"
        };

        var dto = new UpdateSubjectDto
        {
            Id = 5,
            Name = "New Name"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(existing);

        await _service.UpdateAsync(dto);

        _repositoryMock.Verify(r => r.UpdateAsync(existing), Times.Once);
        Assert.Equal("New Name", existing.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenSubjectDoesNotExist_ThrowsKeyNotFoundException()
    {
        var dto = new UpdateSubjectDto
        {
            Id = 100,
            Name = "Unknown"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(100))
            .ReturnsAsync((Subjects?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(dto));
    }

    [Fact]
    public async Task DeleteAsync_WhenCalled_CallsRepository()
    {
        const int subjectId = 7;

        await _service.DeleteAsync(subjectId);

        _repositoryMock.Verify(r => r.DeleteAsync(subjectId), Times.Once);
    }
}