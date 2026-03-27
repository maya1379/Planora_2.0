using Planora.Domain.Enums;
using Planora.Services.DTOs;
using Xunit;

namespace Planora.Tests;

public class ScheduleDtosTests
{
    [Fact]
    public void ScheduleEntryDto_DefaultValues_AreCorrect()
    {
        var dto = new ScheduleEntryDto();

        Assert.Equal(0, dto.Id);
        Assert.Equal(default, dto.DayOfWeek);
        Assert.Equal(default, dto.WeekType);
        Assert.Equal(0, dto.TimeSlotId);
        Assert.Equal(0, dto.TimeSlotNumber);
        Assert.Equal(default, dto.StartTime);
        Assert.Equal(default, dto.EndTime);
        Assert.Equal(0, dto.ClassroomId);
        Assert.Equal(string.Empty, dto.ClassroomNumber);
        Assert.Equal(string.Empty, dto.BuildingName);
        Assert.Equal(string.Empty, dto.TeacherId);
        Assert.Equal(string.Empty, dto.TeacherName);
        Assert.Equal(0, dto.SubjectId);
        Assert.Equal(string.Empty, dto.SubjectName);
        Assert.Equal(default, dto.LessonType);
        Assert.Equal(0, dto.GroupId);
        Assert.Equal(string.Empty, dto.GroupName);
    }

    [Fact]
    public void ScheduleEntryDto_Properties_CanBeAssigned()
    {
        var dto = new ScheduleEntryDto
        {
            Id = 1,
            DayOfWeek = DayOfWeekEnum.Monday,
            WeekType = WeekType.Numerator,
            TimeSlotId = 2,
            TimeSlotNumber = 3,
            StartTime = new TimeSpan(10, 25, 0),
            EndTime = new TimeSpan(12, 0, 0),
            ClassroomId = 4,
            ClassroomNumber = "205",
            BuildingName = "Main Building",
            TeacherId = "teacher1",
            TeacherName = "Ivan Ivanov",
            SubjectId = 7,
            SubjectName = "Programming",
            LessonType = LessonType.Lecture,
            GroupId = 9,
            GroupName = "IPZ-21"
        };

        Assert.Equal(1, dto.Id);
        Assert.Equal(DayOfWeekEnum.Monday, dto.DayOfWeek);
        Assert.Equal(WeekType.Numerator, dto.WeekType);
        Assert.Equal(2, dto.TimeSlotId);
        Assert.Equal(3, dto.TimeSlotNumber);
        Assert.Equal(new TimeSpan(10, 25, 0), dto.StartTime);
        Assert.Equal(new TimeSpan(12, 0, 0), dto.EndTime);
        Assert.Equal(4, dto.ClassroomId);
        Assert.Equal("205", dto.ClassroomNumber);
        Assert.Equal("Main Building", dto.BuildingName);
        Assert.Equal("teacher1", dto.TeacherId);
        Assert.Equal("Ivan Ivanov", dto.TeacherName);
        Assert.Equal(7, dto.SubjectId);
        Assert.Equal("Programming", dto.SubjectName);
        Assert.Equal(LessonType.Lecture, dto.LessonType);
        Assert.Equal(9, dto.GroupId);
        Assert.Equal("IPZ-21", dto.GroupName);
    }

    [Fact]
    public void CreateScheduleEntryDto_DefaultValues_AreCorrect()
    {
        var dto = new CreateScheduleEntryDto();

        Assert.Equal(default, dto.DayOfWeek);
        Assert.Equal(default, dto.WeekType);
        Assert.Equal(0, dto.TimeSlotId);
        Assert.Equal(0, dto.ClassroomId);
        Assert.Equal(string.Empty, dto.TeacherId);
        Assert.Equal(0, dto.SubjectId);
        Assert.Equal(0, dto.GroupId);
    }

    [Fact]
    public void CreateScheduleEntryDto_Properties_CanBeAssigned()
    {
        var dto = new CreateScheduleEntryDto
        {
            DayOfWeek = DayOfWeekEnum.Friday,
            WeekType = WeekType.Both,
            TimeSlotId = 3,
            ClassroomId = 10,
            TeacherId = "teacher2",
            SubjectId = 6,
            GroupId = 8
        };

        Assert.Equal(DayOfWeekEnum.Friday, dto.DayOfWeek);
        Assert.Equal(WeekType.Both, dto.WeekType);
        Assert.Equal(3, dto.TimeSlotId);
        Assert.Equal(10, dto.ClassroomId);
        Assert.Equal("teacher2", dto.TeacherId);
        Assert.Equal(6, dto.SubjectId);
        Assert.Equal(8, dto.GroupId);
    }

    [Fact]
    public void ScheduleGenerationResultDto_DefaultValues_AreCorrect()
    {
        var dto = new ScheduleGenerationResultDto();

        Assert.False(dto.Success);
        Assert.Equal(0, dto.TotalEntriesCreated);
        Assert.NotNull(dto.Warnings);
        Assert.Empty(dto.Warnings);
        Assert.NotNull(dto.Errors);
        Assert.Empty(dto.Errors);
        Assert.NotNull(dto.Entries);
        Assert.Empty(dto.Entries);
    }

    [Fact]
    public void ScheduleGenerationResultDto_Properties_CanBeAssigned()
    {
        var dto = new ScheduleGenerationResultDto
        {
            Success = true,
            TotalEntriesCreated = 12,
            Warnings = new List<string> { "Warning 1" },
            Errors = new List<string> { "Error 1" },
            Entries = new List<ScheduleEntryDto>
            {
                new ScheduleEntryDto { Id = 1 }
            }
        };

        Assert.True(dto.Success);
        Assert.Equal(12, dto.TotalEntriesCreated);
        Assert.Single(dto.Warnings);
        Assert.Single(dto.Errors);
        Assert.Single(dto.Entries);
        Assert.Equal(1, dto.Entries[0].Id);
    }
}