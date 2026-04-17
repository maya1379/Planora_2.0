using Planora.Services.DTOs;
using Xunit;

namespace Planora.Tests;

public class TimeSlotDtosTests
{
    [Fact]
    public void TimeSlotDto_DefaultValues_AreCorrect()
    {
        var dto = new TimeSlotDto();

        Assert.Equal(0, dto.Id);
        Assert.Equal(0, dto.Number);
        Assert.Equal(default, dto.StartTime);
        Assert.Equal(default, dto.EndTime);
    }

    [Fact]
    public void TimeSlotDto_Properties_CanBeAssigned()
    {
        var dto = new TimeSlotDto
        {
            Id = 1,
            Number = 2,
            StartTime = new TimeSpan(10, 25, 0),
            EndTime = new TimeSpan(12, 0, 0)
        };

        Assert.Equal(1, dto.Id);
        Assert.Equal(2, dto.Number);
        Assert.Equal(new TimeSpan(10, 25, 0), dto.StartTime);
        Assert.Equal(new TimeSpan(12, 0, 0), dto.EndTime);
    }

    [Fact]
    public void CreateTimeSlotDto_DefaultValues_AreCorrect()
    {
        var dto = new CreateTimeSlotDto();

        Assert.Equal(0, dto.Number);
        Assert.Equal(default, dto.StartTime);
        Assert.Equal(default, dto.EndTime);
    }

    [Fact]
    public void CreateTimeSlotDto_Properties_CanBeAssigned()
    {
        var dto = new CreateTimeSlotDto
        {
            Number = 4,
            StartTime = new TimeSpan(12, 20, 0),
            EndTime = new TimeSpan(13, 55, 0)
        };

        Assert.Equal(4, dto.Number);
        Assert.Equal(new TimeSpan(12, 20, 0), dto.StartTime);
        Assert.Equal(new TimeSpan(13, 55, 0), dto.EndTime);
    }
}