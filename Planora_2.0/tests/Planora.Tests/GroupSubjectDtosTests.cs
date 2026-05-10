using Planora.Domain.Constants;
using Planora.Domain.Enums;
using Planora.Services.DTOs;
using Xunit;

namespace Planora.Tests;

public class GroupSubjectDtosTests
{
    [Fact]
    public void GroupSubjectDto_DefaultValues_AreCorrect()
    {
        var dto = new GroupSubjectDto();

        Assert.Equal(0, dto.Id);
        Assert.Equal(0, dto.GroupId);
        Assert.Equal(string.Empty, dto.GroupName);
        Assert.Equal(0, dto.SubjectId);
        Assert.Equal(string.Empty, dto.SubjectName);
        Assert.Equal(0, dto.HoursPerWeek);
        Assert.Equal(default, dto.LessonType);
    }

    [Fact]
    public void GroupSubjectDto_Properties_CanBeAssigned()
    {
        var dto = new GroupSubjectDto
        {
            Id = 1,
            GroupId = 10,
            GroupName = "IPZ-21",
            SubjectId = 20,
            SubjectName = "Programming",
            HoursPerWeek = 4,
            LessonType = LessonType.Lecture
        };

        Assert.Equal(1, dto.Id);
        Assert.Equal(10, dto.GroupId);
        Assert.Equal("IPZ-21", dto.GroupName);
        Assert.Equal(20, dto.SubjectId);
        Assert.Equal("Programming", dto.SubjectName);
        Assert.Equal(4, dto.HoursPerWeek);
        Assert.Equal(LessonType.Lecture, dto.LessonType);
    }

    [Fact]
    public void CreateGroupSubjectDto_DefaultValues_AreCorrect()
    {
        var dto = new CreateGroupSubjectDto();

        Assert.Equal(0, dto.GroupId);
        Assert.Equal(0, dto.SubjectId);
        Assert.Equal(0, dto.HoursPerWeek);
        Assert.Equal(default, dto.LessonType);
    }

    [Fact]
    public void CreateGroupSubjectDto_Properties_CanBeAssigned()
    {
        var dto = new CreateGroupSubjectDto
        {
            GroupId = 5,
            SubjectId = 7,
            HoursPerWeek = 3,
            LessonType = LessonType.Practice
        };

        Assert.Equal(5, dto.GroupId);
        Assert.Equal(7, dto.SubjectId);
        Assert.Equal(3, dto.HoursPerWeek);
        Assert.Equal(LessonType.Practice, dto.LessonType);
    }

    [Fact]
    public void UpdateGroupSubjectDto_DefaultValues_AreCorrect()
    {
        var dto = new UpdateGroupSubjectDto();

        Assert.Equal(0, dto.Id);
        Assert.Equal(0, dto.GroupId);
        Assert.Equal(0, dto.SubjectId);
        Assert.Equal(0, dto.HoursPerWeek);
        Assert.Equal(default, dto.LessonType);
    }

    [Fact]
    public void UpdateGroupSubjectDto_Properties_CanBeAssigned()
    {
        var dto = new UpdateGroupSubjectDto
        {
            Id = 12,
            GroupId = 3,
            SubjectId = 9,
            HoursPerWeek = 2,
            LessonType = LessonType.Lab
        };

        Assert.Equal(12, dto.Id);
        Assert.Equal(3, dto.GroupId);
        Assert.Equal(9, dto.SubjectId);
        Assert.Equal(2, dto.HoursPerWeek);
        Assert.Equal(LessonType.Lab, dto.LessonType);
    }
}