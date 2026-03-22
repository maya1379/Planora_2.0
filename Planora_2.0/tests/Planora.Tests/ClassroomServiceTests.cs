using AutoMapper;
using Moq;
using Planora.Domain.Entities;
using Planora.Domain.Enums;
using Planora.Services.DTOs;
using Planora.Services.Interfaces;
using Planora.Services.Mapping;
using Planora.Services.Services;
using Xunit;

namespace Planora.Tests;

public class ClassroomServiceTests
{
    private readonly Mock<IClassroomRepository> _classroomRepositoryMock;
    private readonly Mock<ITimeSlotRepository> _timeSlotRepositoryMock;
    private readonly IMapper _mapper;
    private readonly ClassroomService _service;

    public ClassroomServiceTests()
    {
        _classroomRepositoryMock = new Mock<IClassroomRepository>();
        _timeSlotRepositoryMock = new Mock<ITimeSlotRepository>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        _mapper = config.CreateMapper();

        _service = new ClassroomService(
            _classroomRepositoryMock.Object,
            _timeSlotRepositoryMock.Object,
            _mapper);
    }

    [Fact]
    public async Task GetAllAsync_WhenClassroomsExist_ReturnsMappedDtos()
    {
        // Arrange
        var classrooms = new List<Classrooms>
        {
            new Classrooms
            {
                Id = 1,
                Number = "101",
                Building = new Building { Name = "Main корпус" }
            },
            new Classrooms
            {
                Id = 2,
                Number = "102",
                Building = new Building { Name = "Main корпус" }
            }
        };

        _classroomRepositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(classrooms);

        // Act
        var result = (await _service.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("101", result[0].Number);
        Assert.Equal("Main корпус", result[0].BuildingName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenClassroomExists_ReturnsMappedDto()
    {
        // Arrange
        var classroom = new Classrooms
        {
            Id = 1,
            Number = "201",
            Building = new Building { Name = "B корпус" }
        };

        _classroomRepositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(classroom);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal("201", result.Number);
        Assert.Equal("B корпус", result.BuildingName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenClassroomDoesNotExist_ReturnsNull()
    {
        // Arrange
        _classroomRepositoryMock
            .Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((Classrooms?)null);

        // Act
        var result = await _service.GetByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByBuildingIdAsync_WhenCalled_ReturnsMappedDtos()
    {
        // Arrange
        var classrooms = new List<Classrooms>
        {
            new Classrooms
            {
                Id = 1,
                Number = "301",
                Building = new Building { Name = "C корпус" }
            }
        };

        _classroomRepositoryMock
            .Setup(r => r.GetByBuildingIdAsync(5))
            .ReturnsAsync(classrooms);

        // Act
        var result = (await _service.GetByBuildingIdAsync(5)).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("301", result[0].Number);
        Assert.Equal("C корпус", result[0].BuildingName);
    }

    [Fact]
    public async Task GetAvailableAsync_WhenCalled_ReturnsMappedDtos()
    {
        // Arrange
        var classrooms = new List<Classrooms>
        {
            new Classrooms
            {
                Id = 1,
                Number = "401",
                Building = new Building { Name = "D корпус" }
            }
        };

        _classroomRepositoryMock
            .Setup(r => r.GetAvailableAsync(DayOfWeekEnum.Monday, 2, WeekType.Numerator))
            .ReturnsAsync(classrooms);

        // Act
        var result = (await _service.GetAvailableAsync(DayOfWeekEnum.Monday, 2, WeekType.Numerator)).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("401", result[0].Number);
    }

    [Fact]
    public async Task CreateAsync_WhenDtoIsValid_ReturnsCreatedDto()
    {
        // Arrange
        var dto = new CreateClassroomDto();

        var createdClassroom = new Classrooms
        {
            Id = 10,
            Number = "501",
            Building = new Building { Name = "E корпус" }
        };

        _classroomRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Classrooms>()))
            .ReturnsAsync(createdClassroom);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("501", result.Number);
        Assert.Equal("E корпус", result.BuildingName);
    }

    [Fact]
    public async Task UpdateAsync_WhenClassroomExists_CallsRepositoryUpdate()
    {
        // Arrange
        var existing = new Classrooms
        {
            Id = 7,
            Number = "601",
            Building = new Building { Name = "F корпус" }
        };

        var dto = new UpdateClassroomDto
        {
            Id = 7
        };

        _classroomRepositoryMock
            .Setup(r => r.GetByIdAsync(7))
            .ReturnsAsync(existing);

        // Act
        await _service.UpdateAsync(dto);

        // Assert
        _classroomRepositoryMock.Verify(r => r.UpdateAsync(existing), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenClassroomDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var dto = new UpdateClassroomDto
        {
            Id = 77
        };

        _classroomRepositoryMock
            .Setup(r => r.GetByIdAsync(77))
            .ReturnsAsync((Classrooms?)null);

        // Act + Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(dto));
    }

    [Fact]
    public async Task DeleteAsync_WhenCalled_CallsRepositoryDelete()
    {
        // Arrange
        const int classroomId = 3;

        // Act
        await _service.DeleteAsync(classroomId);

        // Assert
        _classroomRepositoryMock.Verify(r => r.DeleteAsync(classroomId), Times.Once);
    }

    [Theory]
    [InlineData(System.DayOfWeek.Monday, DayOfWeekEnum.Monday)]
    [InlineData(System.DayOfWeek.Tuesday, DayOfWeekEnum.Tuesday)]
    [InlineData(System.DayOfWeek.Wednesday, DayOfWeekEnum.Wednesday)]
    [InlineData(System.DayOfWeek.Thursday, DayOfWeekEnum.Thursday)]
    [InlineData(System.DayOfWeek.Friday, DayOfWeekEnum.Friday)]
    [InlineData(System.DayOfWeek.Saturday, DayOfWeekEnum.Saturday)]
    public void GetCurrentDayOfWeekEnum_WhenWeekdayPassed_ReturnsExpectedEnum(
        System.DayOfWeek input,
        DayOfWeekEnum expected)
    {
        // Act
        var result = ClassroomService.GetCurrentDayOfWeekEnum(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetCurrentDayOfWeekEnum_WhenSundayPassed_ReturnsNull()
    {
        // Act
        var result = ClassroomService.GetCurrentDayOfWeekEnum(System.DayOfWeek.Sunday);

        // Assert
        Assert.Null(result);
    }
}