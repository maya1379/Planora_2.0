using AutoMapper;
using Moq;
using Planora.Domain.Entities;
using Planora.Services.DTOs;
using Planora.Services.Interfaces;
using Planora.Services.Mapping;
using Planora.Services.Services;
using Xunit;

namespace Planora.Tests;

public class GroupSubjectServiceTests
{
    private readonly Mock<IGroupSubjectRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly GroupSubjectService _service;

    public GroupSubjectServiceTests()
    {
        _repositoryMock = new Mock<IGroupSubjectRepository>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        _mapper = config.CreateMapper();

        _service = new GroupSubjectService(_repositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAllAsync_WhenDataExists_ReturnsMappedDtos()
    {
        // Arrange
        var data = new List<GroupDisciplineList>
        {
            new GroupDisciplineList
            {
                Id = 1,
                Groups = new Groups { Name = "Group A" },
                Subjects = new Subjects { Name = "Math" }
            }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(data);

        // Act
        var result = (await _service.GetAllAsync()).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Group A", result[0].GroupName);
        Assert.Equal("Math", result[0].SubjectName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsDto()
    {
        // Arrange
        var entity = new GroupDisciplineList
        {
            Id = 2,
            Groups = new Groups { Name = "Group B" },
            Subjects = new Subjects { Name = "Physics" }
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(entity);

        // Act
        var result = await _service.GetByIdAsync(2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Group B", result!.GroupName);
        Assert.Equal("Physics", result.SubjectName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((GroupDisciplineList?)null);

        // Act
        var result = await _service.GetByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByGroupIdAsync_WhenCalled_ReturnsDtos()
    {
        // Arrange
        var data = new List<GroupDisciplineList>
        {
            new GroupDisciplineList
            {
                Id = 3,
                Groups = new Groups { Name = "Group C" },
                Subjects = new Subjects { Name = "Chemistry" }
            }
        };

        _repositoryMock.Setup(r => r.GetByGroupIdAsync(5)).ReturnsAsync(data);

        // Act
        var result = (await _service.GetByGroupIdAsync(5)).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Group C", result[0].GroupName);
    }

    [Fact]
    public async Task CreateAsync_WhenValidDto_ReturnsCreatedDto()
    {
        // Arrange
        var dto = new CreateGroupSubjectDto();

        var created = new GroupDisciplineList
        {
            Id = 10,
            Groups = new Groups { Name = "Group D" },
            Subjects = new Subjects { Name = "Biology" }
        };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<GroupDisciplineList>()))
            .ReturnsAsync(created);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("Group D", result.GroupName);
        Assert.Equal("Biology", result.SubjectName);
    }

    [Fact]
    public async Task UpdateAsync_WhenExists_CallsUpdate()
    {
        // Arrange
        var existing = new GroupDisciplineList { Id = 5 };

        var dto = new UpdateGroupSubjectDto
        {
            Id = 5
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(existing);

        // Act
        await _service.UpdateAsync(dto);

        // Assert
        _repositoryMock.Verify(r => r.UpdateAsync(existing), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotExists_ThrowsException()
    {
        // Arrange
        var dto = new UpdateGroupSubjectDto { Id = 100 };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(100))
            .ReturnsAsync((GroupDisciplineList?)null);

        // Act + Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(dto));
    }

    [Fact]
    public async Task DeleteAsync_WhenCalled_CallsRepository()
    {
        // Arrange
        int id = 7;

        // Act
        await _service.DeleteAsync(id);

        // Assert00000000
        _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
    }
}