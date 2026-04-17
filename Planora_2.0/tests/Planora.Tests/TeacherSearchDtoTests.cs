using Planora.Services.DTOs;
using Xunit;

namespace Planora.Tests;

public class TeacherSearchDtoTests
{
    [Fact]
    public void TeacherSearchDto_DefaultValues_AreCorrect()
    {
        var dto = new TeacherSearchDto();

        Assert.Equal(string.Empty, dto.Id);
        Assert.Equal(string.Empty, dto.FullName);
        Assert.Null(dto.Faculty);
        Assert.Null(dto.Position);
        Assert.Null(dto.Email);
    }

    [Fact]
    public void TeacherSearchDto_Properties_CanBeAssigned()
    {
        var dto = new TeacherSearchDto
        {
            Id = "t1",
            FullName = "Ivan Ivanov",
            Faculty = "FIT",
            Position = "Assistant",
            Email = "ivan@test.com"
        };

        Assert.Equal("t1", dto.Id);
        Assert.Equal("Ivan Ivanov", dto.FullName);
        Assert.Equal("FIT", dto.Faculty);
        Assert.Equal("Assistant", dto.Position);
        Assert.Equal("ivan@test.com", dto.Email);
    }

    [Fact]
    public void TeacherSearchDto_NullableProperties_CanBeNull()
    {
        var dto = new TeacherSearchDto
        {
            Id = "t2",
            FullName = "Petro Petrenko",
            Faculty = null,
            Position = null,
            Email = null
        };

        Assert.Equal("t2", dto.Id);
        Assert.Equal("Petro Petrenko", dto.FullName);
        Assert.Null(dto.Faculty);
        Assert.Null(dto.Position);
        Assert.Null(dto.Email);
    }
}