using AutoMapper;
using Moq;
using Planora.Domain.Entities;
using Planora.Domain.Constants;
using Planora.Domain.Enums;
using Planora.Services.DTOs;
using Planora.Services.Interfaces;
using Planora.Services.Services;
using Xunit;

namespace Planora.Tests;

public class ScheduleGenerationServiceTests
{
    private readonly Mock<IGroupRepository> _groupRepositoryMock;
    private readonly Mock<IGroupSubjectRepository> _groupSubjectRepositoryMock;
    private readonly Mock<ITeachingAssignmentRepository> _teachingAssignmentRepositoryMock;
    private readonly Mock<IClassroomRepository> _classroomRepositoryMock;
    private readonly Mock<ITimeSlotRepository> _timeSlotRepositoryMock;
    private readonly Mock<IScheduleEntryRepository> _scheduleEntryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;

    private readonly ScheduleGenerationService _service;

    public ScheduleGenerationServiceTests()
    {
        _groupRepositoryMock = new Mock<IGroupRepository>();
        _groupSubjectRepositoryMock = new Mock<IGroupSubjectRepository>();
        _teachingAssignmentRepositoryMock = new Mock<ITeachingAssignmentRepository>();
        _classroomRepositoryMock = new Mock<IClassroomRepository>();
        _timeSlotRepositoryMock = new Mock<ITimeSlotRepository>();
        _scheduleEntryRepositoryMock = new Mock<IScheduleEntryRepository>();
        _mapperMock = new Mock<IMapper>();

        _service = new ScheduleGenerationService(
            _groupRepositoryMock.Object,
            _groupSubjectRepositoryMock.Object,
            _teachingAssignmentRepositoryMock.Object,
            _classroomRepositoryMock.Object,
            _timeSlotRepositoryMock.Object,
            _scheduleEntryRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GenerateScheduleAsync_WhenNoGroups_ReturnsError()
    {
        _groupRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Groups>());

        var result = await _service.GenerateScheduleAsync();

        Assert.False(result.Success);
        Assert.Contains("Немає жодної групи в системі.", result.Errors);
        _scheduleEntryRepositoryMock.Verify(r => r.DeleteAllAsync(), Times.Never);
    }

    [Fact]
    public async Task GenerateScheduleAsync_WhenNoGroupPlans_ReturnsError()
    {
        _groupRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Groups>
            {
                new Groups { Id = 1, Name = "IPZ-21", StudentCount = 20 }
            });

        _groupSubjectRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<GroupDisciplineList>());

        _teachingAssignmentRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<TeachingAssignment> { new TeachingAssignment() });

        _classroomRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Classrooms> { new Classrooms() });

        _timeSlotRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<TimeSlot> { new TimeSlot { Id = 1, Number = 1 } });

        var result = await _service.GenerateScheduleAsync();

        Assert.False(result.Success);
        Assert.Contains("Немає навчальних планів для груп.", result.Errors);
    }

    [Fact]
    public async Task GenerateScheduleAsync_WhenNoTeachingAssignments_ReturnsError()
    {
        _groupRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Groups>
            {
                new Groups { Id = 1, Name = "IPZ-21", StudentCount = 20 }
            });

        _groupSubjectRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<GroupDisciplineList>
            {
                new GroupDisciplineList { Id = 1, GroupId = 1, SubjectId = 1, HoursPerWeek = 2 }
            });

        _teachingAssignmentRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<TeachingAssignment>());

        _classroomRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Classrooms> { new Classrooms() });

        _timeSlotRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<TimeSlot> { new TimeSlot { Id = 1, Number = 1 } });

        var result = await _service.GenerateScheduleAsync();

        Assert.False(result.Success);
        Assert.Contains("Немає навчального навантаження викладачів.", result.Errors);
    }

    [Fact]
    public async Task GenerateScheduleAsync_WhenNoClassrooms_ReturnsError()
    {
        _groupRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Groups>
            {
                new Groups { Id = 1, Name = "IPZ-21", StudentCount = 20 }
            });

        _groupSubjectRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<GroupDisciplineList>
            {
                new GroupDisciplineList { Id = 1, GroupId = 1, SubjectId = 1, HoursPerWeek = 2 }
            });

        _teachingAssignmentRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<TeachingAssignment>
            {
                new TeachingAssignment { GroupId = 1, SubjectId = 1, TeacherId = "teacher1" }
            });

        _classroomRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Classrooms>());

        _timeSlotRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<TimeSlot> { new TimeSlot { Id = 1, Number = 1 } });

        var result = await _service.GenerateScheduleAsync();

        Assert.False(result.Success);
        Assert.Contains("Немає жодної аудиторії в системі.", result.Errors);
    }

    [Fact]
    public async Task GenerateScheduleAsync_WhenNoTimeSlots_ReturnsError()
    {
        _groupRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Groups>
            {
                new Groups { Id = 1, Name = "IPZ-21", StudentCount = 20 }
            });

        _groupSubjectRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<GroupDisciplineList>
            {
                new GroupDisciplineList { Id = 1, GroupId = 1, SubjectId = 1, HoursPerWeek = 2 }
            });

        _teachingAssignmentRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<TeachingAssignment>
            {
                new TeachingAssignment { GroupId = 1, SubjectId = 1, TeacherId = "teacher1" }
            });

        _classroomRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Classrooms>
            {
                new Classrooms { Id = 1, Capacity = 30, BuildingId = 1 }
            });

        _timeSlotRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<TimeSlot>());

        var result = await _service.GenerateScheduleAsync();

        Assert.False(result.Success);
        Assert.Contains("Немає тайм-слотів (пар) в системі.", result.Errors);
    }

    [Fact]
    public async Task GenerateScheduleAsync_WhenNoMatchingTeachingAssignment_ReturnsErrorAndWarning()
    {
        _groupRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Groups>
            {
                new Groups { Id = 1, Name = "IPZ-21", StudentCount = 20 }
            });

        _groupSubjectRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<GroupDisciplineList>
            {
                new GroupDisciplineList
                {
                    Id = 1,
                    GroupId = 1,
                    SubjectId = 1,
                    HoursPerWeek = 2,
                    LessonType = LessonType.Lecture,
                    Groups = new Groups { Name = "IPZ-21" },
                    Subjects = new Subjects { Name = "Math" }
                }
            });

        _teachingAssignmentRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<TeachingAssignment>
            {
                new TeachingAssignment
                {
                    Id = 99,
                    GroupId = 999,
                    SubjectId = 999,
                    TeacherId = "other-teacher"
                }
            });

        _classroomRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Classrooms>
            {
                new Classrooms { Id = 1, Capacity = 30, BuildingId = 1 }
            });

        _timeSlotRepositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<TimeSlot>
            {
                new TimeSlot { Id = 1, Number = 1 }
            });

        var result = await _service.GenerateScheduleAsync();

        Assert.False(result.Success);
        Assert.Contains(result.Warnings, w => w.Contains("не призначено викладача"));
        Assert.Contains(result.Errors, e => e.Contains("Не вдалося побудувати вимоги до розкладу"));
        _scheduleEntryRepositoryMock.Verify(r => r.DeleteAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GenerateScheduleAsync_WhenValidData_CreatesSchedule()
    {
        var groups = new List<Groups>
        {
            new Groups { Id = 1, Name = "IPZ-21", StudentCount = 20 }
        };

        var plans = new List<GroupDisciplineList>
        {
            new GroupDisciplineList
            {
                Id = 1,
                GroupId = 1,
                SubjectId = 1,
                HoursPerWeek = 2,
                LessonType = LessonType.Lecture,
                Groups = new Groups { Name = "IPZ-21" },
                Subjects = new Subjects { Name = "Programming" }
            }
        };

        var assignments = new List<TeachingAssignment>
        {
            new TeachingAssignment
            {
                Id = 1,
                GroupId = 1,
                SubjectId = 1,
                TeacherId = "teacher1",
                Teacher = new User { FullName = "Ivan Ivanov" }
            }
        };

        var classrooms = new List<Classrooms>
        {
            new Classrooms
            {
                Id = 1,
                Capacity = 30,
                HasComputers = false,
                BuildingId = 1
            }
        };

        var timeSlots = new List<TimeSlot>
        {
            new TimeSlot { Id = 1, Number = 1 },
            new TimeSlot { Id = 2, Number = 2 }
        };

        _groupRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(groups);
        _groupSubjectRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(plans);
        _teachingAssignmentRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(assignments);
        _classroomRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(classrooms);
        _timeSlotRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(timeSlots);

        _scheduleEntryRepositoryMock
            .Setup(r => r.AddRangeAsync(It.IsAny<List<Schedule>>()))
            .Returns(Task.CompletedTask);

        _scheduleEntryRepositoryMock
            .Setup(r => r.DeleteAllAsync())
            .Returns(Task.CompletedTask);

        var result = await _service.GenerateScheduleAsync();

        Assert.True(result.Success);
        Assert.Empty(result.Errors);
        Assert.True(result.TotalEntriesCreated > 0);

        _scheduleEntryRepositoryMock.Verify(r => r.DeleteAllAsync(), Times.Once);
        _scheduleEntryRepositoryMock.Verify(r => r.AddRangeAsync(It.IsAny<List<Schedule>>()), Times.Once);
    }
}