using Planora.Services.DTOs;
using Xunit;

namespace Planora.Tests;

public class GroupDtosTests
{
    [Fact]
    public void GroupDto_DefaultValues_AreCorrect()
    {
        var dto = new GroupDto();

        Assert.Equal(0, dto.Id);
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Faculty);
        Assert.Equal(0, dto.StudentCount);
    }

    [Fact]
    public void GroupDto_Properties_CanBeAssigned()
    {
        var dto = new GroupDto
        {
            Id = 1,
            Name = "IPZ-21",
            Faculty = "FIT",
            StudentCount = 25
        };

        Assert.Equal(1, dto.Id);
        Assert.Equal("IPZ-21", dto.Name);
        Assert.Equal("FIT", dto.Faculty);
        Assert.Equal(25, dto.StudentCount);
    }

    [Fact]
    public void CreateGroupDto_DefaultValues_AreCorrect()
    {
        var dto = new CreateGroupDto();

        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Faculty);
        Assert.Equal(0, dto.StudentCount);
    }

    [Fact]
    public void CreateGroupDto_Properties_CanBeAssigned()
    {
        var dto = new CreateGroupDto
        {
            Name = "KN-22",
            Faculty = "FCS",
            StudentCount = 18
        };

        Assert.Equal("KN-22", dto.Name);
        Assert.Equal("FCS", dto.Faculty);
        Assert.Equal(18, dto.StudentCount);
    }

    [Fact]
    public void UpdateGroupDto_DefaultValues_AreCorrect()
    {
        var dto = new UpdateGroupDto();

        Assert.Equal(0, dto.Id);
        Assert.Equal(string.Empty, dto.Name);
        Assert.Equal(string.Empty, dto.Faculty);
        Assert.Equal(0, dto.StudentCount);
    }

    [Fact]
    public void UpdateGroupDto_Properties_CanBeAssigned()
    {
        var dto = new UpdateGroupDto
        {
            Id = 5,
            Name = "PI-23",
            Faculty = "Math",
            StudentCount = 30
        };

        Assert.Equal(5, dto.Id);
        Assert.Equal("PI-23", dto.Name);
        Assert.Equal("Math", dto.Faculty);
        Assert.Equal(30, dto.StudentCount);
    }
}