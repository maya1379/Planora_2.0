using Planora.Services.DTOs;
using Xunit;

namespace Planora.Tests;

public class TeachingAssignmentDtosTests
{
    [Fact]
    public void TeachingAssignmentDto_DefaultValues_AreCorrect()
    {
        var dto = new TeachingAssignmentDto();

        Assert.Equal(0, dto.Id);
        Assert.Equal(string.Empty, dto.TeacherId);
        Assert.Equal(string.Empty, dto.TeacherName);
        Assert.Equal(0, dto.SubjectId);
        Assert.Equal(string.Empty, dto.SubjectName);
        Assert.Equal(0, dto.GroupId);
        Assert.Equal(string.Empty, dto.GroupName);
        Assert.Equal(0, dto.HoursPerWeek);
    }

    [Fact]
    public void TeachingAssignmentDto_Properties_CanBeAssigned()
    {
        var dto = new TeachingAssignmentDto
        {
            Id = 1,
            TeacherId = "t1",
            TeacherName = "Ivan Ivanov",
            SubjectId = 2,
            SubjectName = "Math",
            GroupId = 3,
            GroupName = "IPZ-21",
            HoursPerWeek = 4
        };

        Assert.Equal(1, dto.Id);
        Assert.Equal("t1", dto.TeacherId);
        Assert.Equal("Ivan Ivanov", dto.TeacherName);
        Assert.Equal(2, dto.SubjectId);
        Assert.Equal("Math", dto.SubjectName);
        Assert.Equal(3, dto.GroupId);
        Assert.Equal("IPZ-21", dto.GroupName);
        Assert.Equal(4, dto.HoursPerWeek);
    }

    [Fact]
    public void CreateTeachingAssignmentDto_DefaultValues_AreCorrect()
    {
        var dto = new CreateTeachingAssignmentDto();

        Assert.Equal(string.Empty, dto.TeacherId);
        Assert.Equal(0, dto.SubjectId);
        Assert.Equal(0, dto.GroupId);
        Assert.Equal(0, dto.HoursPerWeek);
    }

    [Fact]
    public void CreateTeachingAssignmentDto_Properties_CanBeAssigned()
    {
        var dto = new CreateTeachingAssignmentDto
        {
            TeacherId = "t2",
            SubjectId = 4,
            GroupId = 5,
            HoursPerWeek = 2
        };

        Assert.Equal("t2", dto.TeacherId);
        Assert.Equal(4, dto.SubjectId);
        Assert.Equal(5, dto.GroupId);
        Assert.Equal(2, dto.HoursPerWeek);
    }

    [Fact]
    public void UpdateTeachingAssignmentDto_DefaultValues_AreCorrect()
    {
        var dto = new UpdateTeachingAssignmentDto();

        Assert.Equal(0, dto.Id);
        Assert.Equal(string.Empty, dto.TeacherId);
        Assert.Equal(0, dto.SubjectId);
        Assert.Equal(0, dto.GroupId);
        Assert.Equal(0, dto.HoursPerWeek);
    }

    [Fact]
    public void UpdateTeachingAssignmentDto_Properties_CanBeAssigned()
    {
        var dto = new UpdateTeachingAssignmentDto
        {
            Id = 10,
            TeacherId = "t3",
            SubjectId = 7,
            GroupId = 8,
            HoursPerWeek = 6
        };

        Assert.Equal(10, dto.Id);
        Assert.Equal("t3", dto.TeacherId);
        Assert.Equal(7, dto.SubjectId);
        Assert.Equal(8, dto.GroupId);
        Assert.Equal(6, dto.HoursPerWeek);
    }
}