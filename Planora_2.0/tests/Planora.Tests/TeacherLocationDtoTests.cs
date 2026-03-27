using Planora.Services.DTOs;
using Xunit;

namespace Planora.Tests;

public class TeacherLocationDtoTests
{
    [Fact]
    public void TeacherLocationDto_DefaultValues_AreCorrect()
    {
        var dto = new TeacherLocationDto();

        Assert.Equal(string.Empty, dto.TeacherId);
        Assert.Equal(string.Empty, dto.TeacherName);
        Assert.False(dto.IsTeachingNow);
        Assert.Null(dto.ClassroomNumber);
        Assert.Null(dto.BuildingName);
        Assert.Null(dto.SubjectName);
        Assert.Null(dto.TimeSlotNumber);
        Assert.Null(dto.StartTime);
        Assert.Null(dto.EndTime);
        Assert.Null(dto.GroupName);
    }

    [Fact]
    public void TeacherLocationDto_Properties_CanBeAssigned()
    {
        var dto = new TeacherLocationDto
        {
            TeacherId = "t1",
            TeacherName = "Ivan Ivanov",
            IsTeachingNow = true,
            ClassroomNumber = "205",
            BuildingName = "Main Building",
            SubjectName = "Programming",
            TimeSlotNumber = 3,
            StartTime = new TimeSpan(10, 25, 0),
            EndTime = new TimeSpan(12, 0, 0),
            GroupName = "IPZ-21"
        };

        Assert.Equal("t1", dto.TeacherId);
        Assert.Equal("Ivan Ivanov", dto.TeacherName);
        Assert.True(dto.IsTeachingNow);
        Assert.Equal("205", dto.ClassroomNumber);
        Assert.Equal("Main Building", dto.BuildingName);
        Assert.Equal("Programming", dto.SubjectName);
        Assert.Equal(3, dto.TimeSlotNumber);
        Assert.Equal(new TimeSpan(10, 25, 0), dto.StartTime);
        Assert.Equal(new TimeSpan(12, 0, 0), dto.EndTime);
        Assert.Equal("IPZ-21", dto.GroupName);
    }

    [Fact]
    public void TeacherLocationDto_NullableProperties_CanBeNull()
    {
        var dto = new TeacherLocationDto
        {
            TeacherId = "t2",
            TeacherName = "Petro Petrenko",
            IsTeachingNow = false,
            ClassroomNumber = null,
            BuildingName = null,
            SubjectName = null,
            TimeSlotNumber = null,
            StartTime = null,
            EndTime = null,
            GroupName = null
        };

        Assert.Equal("t2", dto.TeacherId);
        Assert.Equal("Petro Petrenko", dto.TeacherName);
        Assert.False(dto.IsTeachingNow);
        Assert.Null(dto.ClassroomNumber);
        Assert.Null(dto.BuildingName);
        Assert.Null(dto.SubjectName);
        Assert.Null(dto.TimeSlotNumber);
        Assert.Null(dto.StartTime);
        Assert.Null(dto.EndTime);
        Assert.Null(dto.GroupName);
    }
}