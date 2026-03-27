using Planora.Services.DTOs;
using Xunit;

namespace Planora.Tests;

public class ClassroomDtosTests
{
    [Fact]
    public void ClassroomDto_DefaultValues_AreCorrect()
    {
        var dto = new ClassroomDto();

        Assert.Equal(0, dto.Id);
        Assert.Equal(string.Empty, dto.Number);
        Assert.Equal(0, dto.Capacity);
        Assert.False(dto.HasComputers);
        Assert.False(dto.HasProjector);
        Assert.Equal(string.Empty, dto.Faculty);
        Assert.Equal(0, dto.BuildingId);
        Assert.Equal(string.Empty, dto.BuildingName);
    }

    [Fact]
    public void ClassroomDto_Properties_CanBeAssigned()
    {
        var dto = new ClassroomDto
        {
            Id = 1,
            Number = "101",
            Capacity = 30,
            HasComputers = true,
            HasProjector = true,
            Faculty = "FIT",
            BuildingId = 2,
            BuildingName = "Main корпус"
        };

        Assert.Equal(1, dto.Id);
        Assert.Equal("101", dto.Number);
        Assert.Equal(30, dto.Capacity);
        Assert.True(dto.HasComputers);
        Assert.True(dto.HasProjector);
        Assert.Equal("FIT", dto.Faculty);
        Assert.Equal(2, dto.BuildingId);
        Assert.Equal("Main корпус", dto.BuildingName);
    }

    [Fact]
    public void CreateClassroomDto_DefaultValues_AreCorrect()
    {
        var dto = new CreateClassroomDto();

        Assert.Equal(string.Empty, dto.Number);
        Assert.Equal(0, dto.Capacity);
        Assert.False(dto.HasComputers);
        Assert.False(dto.HasProjector);
        Assert.Equal(string.Empty, dto.Faculty);
        Assert.Equal(0, dto.BuildingId);
    }

    [Fact]
    public void CreateClassroomDto_Properties_CanBeAssigned()
    {
        var dto = new CreateClassroomDto
        {
            Number = "202",
            Capacity = 40,
            HasComputers = true,
            HasProjector = false,
            Faculty = "FCS",
            BuildingId = 3
        };

        Assert.Equal("202", dto.Number);
        Assert.Equal(40, dto.Capacity);
        Assert.True(dto.HasComputers);
        Assert.False(dto.HasProjector);
        Assert.Equal("FCS", dto.Faculty);
        Assert.Equal(3, dto.BuildingId);
    }

    [Fact]
    public void UpdateClassroomDto_DefaultValues_AreCorrect()
    {
        var dto = new UpdateClassroomDto();

        Assert.Equal(0, dto.Id);
        Assert.Equal(string.Empty, dto.Number);
        Assert.Equal(0, dto.Capacity);
        Assert.False(dto.HasComputers);
        Assert.False(dto.HasProjector);
        Assert.Equal(string.Empty, dto.Faculty);
        Assert.Equal(0, dto.BuildingId);
    }

    [Fact]
    public void UpdateClassroomDto_Properties_CanBeAssigned()
    {
        var dto = new UpdateClassroomDto
        {
            Id = 10,
            Number = "303",
            Capacity = 25,
            HasComputers = false,
            HasProjector = true,
            Faculty = "Math",
            BuildingId = 4
        };

        Assert.Equal(10, dto.Id);
        Assert.Equal("303", dto.Number);
        Assert.Equal(25, dto.Capacity);
        Assert.False(dto.HasComputers);
        Assert.True(dto.HasProjector);
        Assert.Equal("Math", dto.Faculty);
        Assert.Equal(4, dto.BuildingId);
    }
}