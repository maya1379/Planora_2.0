using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Planora.Domain.Entities;
using Planora.Domain.Enums;
using Planora.Services.DTOs;
using Planora.Services.Interfaces;
using Planora.Services.Services;
using Xunit;

namespace Planora.Tests;

public class ScheduleServiceTests
{
    private readonly Mock<IScheduleEntryRepository> _repositoryMock;
    private readonly Mock<ITimeSlotRepository> _timeSlotRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly ScheduleService _service;

    public ScheduleServiceTests()
    {
        _repositoryMock = new Mock<IScheduleEntryRepository>();
        _timeSlotRepositoryMock = new Mock<ITimeSlotRepository>();
        _mapperMock = new Mock<IMapper>();

        var store = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            store.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);

        _service = new ScheduleService(
            _repositoryMock.Object,
            _timeSlotRepositoryMock.Object,
            _userManagerMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_WhenEntriesExist_ReturnsMappedDtos()
    {
        // Arrange
        var entries = new List<Schedule>
        {
            new Schedule { Id = 1 },
            new Schedule { Id = 2 }
        };

        var dtos = new List<ScheduleEntryDto>
        {
            new ScheduleEntryDto { Id = 1 },
            new ScheduleEntryDto { Id = 2 }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(entries);
        _mapperMock.Setup(m => m.Map<IEnumerable<ScheduleEntryDto>>(entries)).Returns(dtos);

        // Act
        var result = (await _service.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(2, result[1].Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntryExists_ReturnsMappedDto()
    {
        // Arrange
        var entry = new Schedule { Id = 10 };
        var dto = new ScheduleEntryDto { Id = 10 };

        _repositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(entry);
        _mapperMock.Setup(m => m.Map<ScheduleEntryDto>(entry)).Returns(dto);

        // Act
        var result = await _service.GetByIdAsync(10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntryDoesNotExist_ReturnsNull()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Schedule?)null);

        // Act
        var result = await _service.GetByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByGroupIdAsync_WhenCalled_ReturnsMappedDtos()
    {
        // Arrange
        var entries = new List<Schedule> { new Schedule { Id = 1 } };
        var dtos = new List<ScheduleEntryDto> { new ScheduleEntryDto { Id = 1 } };

        _repositoryMock.Setup(r => r.GetByGroupIdAsync(5)).ReturnsAsync(entries);
        _mapperMock.Setup(m => m.Map<IEnumerable<ScheduleEntryDto>>(entries)).Returns(dtos);

        // Act
        var result = (await _service.GetByGroupIdAsync(5)).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
    }

    [Fact]
    public async Task GetByTeacherIdAsync_WhenCalled_ReturnsMappedDtos()
    {
        // Arrange
        var entries = new List<Schedule> { new Schedule { Id = 2 } };
        var dtos = new List<ScheduleEntryDto> { new ScheduleEntryDto { Id = 2 } };

        _repositoryMock.Setup(r => r.GetByTeacherIdAsync("teacher1")).ReturnsAsync(entries);
        _mapperMock.Setup(m => m.Map<IEnumerable<ScheduleEntryDto>>(entries)).Returns(dtos);

        // Act
        var result = (await _service.GetByTeacherIdAsync("teacher1")).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(2, result[0].Id);
    }

    [Fact]
    public async Task GetByClassroomIdAsync_WhenCalled_ReturnsMappedDtos()
    {
        // Arrange
        var entries = new List<Schedule> { new Schedule { Id = 3 } };
        var dtos = new List<ScheduleEntryDto> { new ScheduleEntryDto { Id = 3 } };

        _repositoryMock.Setup(r => r.GetByClassroomIdAsync(8)).ReturnsAsync(entries);
        _mapperMock.Setup(m => m.Map<IEnumerable<ScheduleEntryDto>>(entries)).Returns(dtos);

        // Act
        var result = (await _service.GetByClassroomIdAsync(8)).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(3, result[0].Id);
    }

    [Fact]
    public async Task CreateAsync_WhenDtoIsValid_ReturnsCreatedDto()
    {
        // Arrange
        var dto = new CreateScheduleEntryDto
        {
            DayOfWeek = DayOfWeekEnum.Monday,
            WeekType = WeekType.Numerator,
            TimeSlotId = 1,
            ClassroomId = 1,
            TeacherId = "teacher1",
            SubjectId = 1,
            GroupId = 1
        };

        var entity = new Schedule
        {
            DayOfWeek = DayOfWeekEnum.Monday,
            WeekType = WeekType.Numerator,
            TimeSlotId = 1,
            ClassroomId = 1,
            TeacherId = "teacher1",
            SubjectId = 1,
            GroupId = 1
        };

        var created = new Schedule
        {
            Id = 15,
            DayOfWeek = DayOfWeekEnum.Monday,
            WeekType = WeekType.Numerator,
            TimeSlotId = 1,
            ClassroomId = 1,
            TeacherId = "teacher1",
            SubjectId = 1,
            GroupId = 1
        };

        var createdDto = new ScheduleEntryDto { Id = 15 };

        _mapperMock.Setup(m => m.Map<Schedule>(dto)).Returns(entity);
        _repositoryMock.Setup(r => r.AddAsync(entity)).ReturnsAsync(created);
        _mapperMock.Setup(m => m.Map<ScheduleEntryDto>(created)).Returns(createdDto);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(15, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntryExists_UpdatesRepository()
    {
        // Arrange
        var existing = new Schedule { Id = 4 };

        var dto = new CreateScheduleEntryDto
        {
            DayOfWeek = DayOfWeekEnum.Friday,
            WeekType = WeekType.Both,
            TimeSlotId = 2,
            ClassroomId = 3,
            TeacherId = "teacher2",
            SubjectId = 7,
            GroupId = 9
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(existing);

        // Act
        await _service.UpdateAsync(4, dto);

        // Assert
        Assert.Equal(DayOfWeekEnum.Friday, existing.DayOfWeek);
        Assert.Equal(WeekType.Both, existing.WeekType);
        Assert.Equal(2, existing.TimeSlotId);
        Assert.Equal(3, existing.ClassroomId);
        Assert.Equal("teacher2", existing.TeacherId);
        Assert.Equal(7, existing.SubjectId);
        Assert.Equal(9, existing.GroupId);

        _repositoryMock.Verify(r => r.UpdateAsync(existing), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntryDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var dto = new CreateScheduleEntryDto
        {
            DayOfWeek = DayOfWeekEnum.Monday,
            WeekType = WeekType.Numerator,
            TimeSlotId = 1,
            ClassroomId = 1,
            TeacherId = "teacher1",
            SubjectId = 1,
            GroupId = 1
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(404)).ReturnsAsync((Schedule?)null);

        // Act + Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(404, dto));
    }

    [Fact]
    public async Task DeleteAsync_WhenCalled_CallsRepository()
    {
        // Act
        await _service.DeleteAsync(5);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(5), Times.Once);
    }

    [Fact]
    public async Task DeleteAllAsync_WhenCalled_CallsRepository()
    {
        // Act
        await _service.DeleteAllAsync();

        // Assert
        _repositoryMock.Verify(r => r.DeleteAllAsync(), Times.Once);
    }

    [Fact]
    public async Task SearchTeachersAsync_WhenQueryMatches_ReturnsOnlyTeachers()
    {
        // Arrange
        var users = new List<User>
        {
            new User
            {
                Id = "1",
                FullName = "Ivan Petrenko",
                Faculty = "FIT",
                Position = "Professor",
                Role = UserRole.Teacher
            },
            new User
            {
                Id = "2",
                FullName = "Olena Sidorenko",
                Faculty = "FIT",
                Position = "Assistant",
                Role = UserRole.Teacher
            },
            new User
            {
                Id = "3",
                FullName = "Student User",
                Faculty = "FIT",
                Position = "",
                Role = UserRole.Student
            }
        }.AsQueryable();

        _userManagerMock.Setup(um => um.Users).Returns(users);

        // Act
        var result = (await _service.SearchTeachersAsync("ivan")).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("1", result[0].Id);
        Assert.Equal("Ivan Petrenko", result[0].FullName);
    }

    [Fact]
    public async Task SearchTeachersAsync_WhenQueryIsEmpty_ReturnsAllTeachers()
    {
        // Arrange
        var users = new List<User>
        {
            new User
            {
                Id = "1",
                FullName = "Ivan Petrenko",
                Faculty = "FIT",
                Position = "Professor",
                Role = UserRole.Teacher
            },
            new User
            {
                Id = "2",
                FullName = "Olena Sidorenko",
                Faculty = "FIT",
                Position = "Assistant",
                Role = UserRole.Teacher
            },
            new User
            {
                Id = "3",
                FullName = "Another Student",
                Faculty = "FIT",
                Position = "",
                Role = UserRole.Student
            }
        }.AsQueryable();

        _userManagerMock.Setup(um => um.Users).Returns(users);

        // Act
        var result = (await _service.SearchTeachersAsync("")).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.NotNull(t.FullName));
    }

    [Fact]
    public async Task FindTeacherLocationAsync_WhenTeacherNotFound_ReturnsNull()
    {
        // Arrange
        var users = new List<User>().AsQueryable();
        _userManagerMock.Setup(um => um.Users).Returns(users);

        // Act
        var result = await _service.FindTeacherLocationAsync("missing-teacher");

        // Assert
        Assert.Null(result);
    }
}