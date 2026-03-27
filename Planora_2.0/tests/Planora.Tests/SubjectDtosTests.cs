using Planora.Domain.Enums;
using Planora.Services.DTOs;
using Xunit;

namespace Planora.Tests;

public class SubjectDtosTests
{
    [Fact]
    public void SubjectDto_DefaultValues_AreCorrect()
    {
        var dto = new SubjectDto();

        Assert.Equal(0, dto.Id);
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(default, dto.Type);
        Assert.Null(dto.Requirements);
    }

    [Fact]
    public void SubjectDto_Properties_CanBeAssigned()
    {
        var dto = new SubjectDto
        {
            Id = 1,
            Name = "Mathematics",
            Type = LessonType.Lecture,
            Requirements = "Basic algebra"
        };

        Assert.Equal(1, dto.Id);
        Assert.Equal("Mathematics", dto.Name);
        Assert.Equal(LessonType.Lecture, dto.Type);
        Assert.Equal("Basic algebra", dto.Requirements);
    }

    [Fact]
    public void SubjectDto_Requirements_CanBeNull()
    {
        var dto = new SubjectDto
        {
            Id = 2,
            Name = "Physics",
            Type = LessonType.Practice,
            Requirements = null
        };

        Assert.Equal(2, dto.Id);
        Assert.Equal("Physics", dto.Name);
        Assert.Equal(LessonType.Practice, dto.Type);
        Assert.Null(dto.Requirements);
    }

    [Fact]
    public void CreateSubjectDto_DefaultValues_AreCorrect()
    {
        var dto = new CreateSubjectDto();

        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(default, dto.Type);
        Assert.Null(dto.Requirements);
    }

    [Fact]
    public void CreateSubjectDto_Properties_CanBeAssigned()
    {
        var dto = new CreateSubjectDto
        {
            Name = "Programming",
            Type = LessonType.Lab,
            Requirements = "Computer basics"
        };

        Assert.Equal("Programming", dto.Name);
        Assert.Equal(LessonType.Lab, dto.Type);
        Assert.Equal("Computer basics", dto.Requirements);
    }

    [Fact]
    public void CreateSubjectDto_Requirements_CanBeNull()
    {
        var dto = new CreateSubjectDto
        {
            Name = "Databases",
            Type = LessonType.Lecture,
            Requirements = null
        };

        Assert.Equal("Databases", dto.Name);
        Assert.Equal(LessonType.Lecture, dto.Type);
        Assert.Null(dto.Requirements);
    }

    [Fact]
    public void UpdateSubjectDto_DefaultValues_AreCorrect()
    {
        var dto = new UpdateSubjectDto();

        Assert.Equal(0, dto.Id);
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(default, dto.Type);
        Assert.Null(dto.Requirements);
    }

    [Fact]
    public void UpdateSubjectDto_Properties_CanBeAssigned()
    {
        var dto = new UpdateSubjectDto
        {
            Id = 10,
            Name = "Algorithms",
            Type = LessonType.Practice,
            Requirements = "Programming basics"
        };

        Assert.Equal(10, dto.Id);
        Assert.Equal("Algorithms", dto.Name);
        Assert.Equal(LessonType.Practice, dto.Type);
        Assert.Equal("Programming basics", dto.Requirements);
    }

    [Fact]
    public void UpdateSubjectDto_Requirements_CanBeNull()
    {
        var dto = new UpdateSubjectDto
        {
            Id = 11,
            Name = "Networks",
            Type = LessonType.Lecture,
            Requirements = null
        };

        Assert.Equal(11, dto.Id);
        Assert.Equal("Networks", dto.Name);
        Assert.Equal(LessonType.Lecture, dto.Type);
        Assert.Null(dto.Requirements);
    }
}