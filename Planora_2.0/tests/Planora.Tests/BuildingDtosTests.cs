using Planora.Services.DTOs;
using Xunit;

namespace Planora.Tests;

public class BuildingDtosTests
{
    [Fact]
    public void BuildingDto_DefaultValues_AreCorrect()
    {
        var dto = new BuildingDto();

        Assert.Equal(0, dto.Id);
        Assert.Equal(string.Empty, dto.Name);
        Assert.Null(dto.Address);
        Assert.Equal(0, dto.ClassroomCount);
    }

    [Fact]
    public void BuildingDto_Properties_CanBeAssigned()
    {
        var dto = new BuildingDto
        {
            Id = 1,
            Name = "Main Building",
            Address = "Street 1",
            ClassroomCount = 15
        };

        Assert.Equal(1, dto.Id);
        Assert.Equal("Main Building", dto.Name);
        Assert.Equal("Street 1", dto.Address);
        Assert.Equal(15, dto.ClassroomCount);
    }

    [Fact]
    public void CreateBuildingDto_DefaultValues_AreCorrect()
    {
        var dto = new CreateBuildingDto();

        Assert.Equal(string.Empty, dto.Name);
        Assert.Null(dto.Address);
    }

    [Fact]
    public void CreateBuildingDto_Properties_CanBeAssigned()
    {
        var dto = new CreateBuildingDto
        {
            Name = "New Building",
            Address = "Street 2"
        };

        Assert.Equal("New Building", dto.Name);
        Assert.Equal("Street 2", dto.Address);
    }

    [Fact]
    public void CreateBuildingDto_Address_CanBeNull()
    {
        var dto = new CreateBuildingDto
        {
            Name = "New Building",
            Address = null
        };

        Assert.Equal("New Building", dto.Name);
        Assert.Null(dto.Address);
    }

    [Fact]
    public void UpdateBuildingDto_DefaultValues_AreCorrect()
    {
        var dto = new UpdateBuildingDto();

        Assert.Equal(0, dto.Id);
        Assert.Equal(string.Empty, dto.Name);
        Assert.Null(dto.Address);
    }

    [Fact]
    public void UpdateBuildingDto_Properties_CanBeAssigned()
    {
        var dto = new UpdateBuildingDto
        {
            Id = 5,
            Name = "Updated Building",
            Address = "Updated Address"
        };

        Assert.Equal(5, dto.Id);
        Assert.Equal("Updated Building", dto.Name);
        Assert.Equal("Updated Address", dto.Address);
    }

    [Fact]
    public void UpdateBuildingDto_Address_CanBeNull()
    {
        var dto = new UpdateBuildingDto
        {
            Id = 5,
            Name = "Updated Building",
            Address = null
        };

        Assert.Equal(5, dto.Id);
        Assert.Equal("Updated Building", dto.Name);
        Assert.Null(dto.Address);
    }
}