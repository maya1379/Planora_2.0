using AutoMapper;
using Moq;
using Planora.Domain.Entities;
using Planora.Services.DTOs;
using Planora.Services.Interfaces;
using Planora.Services.Mapping;
using Planora.Services.Services;
using Xunit;

namespace Planora.Tests;

public class TimeSlotServiceTests
{
    private readonly Mock<ITimeSlotRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly TimeSlotService _service;

    public TimeSlotServiceTests()
    {
        _repositoryMock = new Mock<ITimeSlotRepository>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        _mapper = config.CreateMapper();
        _service = new TimeSlotService(_repositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAllAsync_WhenTimeSlotsExist_ReturnsDtos()
    {
        var data = new List<TimeSlot>
        {
            new TimeSlot
            {
                Id = 1,
                Number = 1,
                StartTime = new TimeSpan(8, 30, 0),
                EndTime = new TimeSpan(10, 5, 0)
            },
            new TimeSlot
            {
                Id = 2,
                Number = 2,
                StartTime = new TimeSpan(10, 25, 0),
                EndTime = new TimeSpan(12, 0, 0)
            }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(data);

        var result = (await _service.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Number);
        Assert.Equal(new TimeSpan(8, 30, 0), result[0].StartTime);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsDto()
    {
        var entity = new TimeSlot
        {
            Id = 1,
            Number = 3,
            StartTime = new TimeSpan(12, 20, 0),
            EndTime = new TimeSpan(13, 55, 0)
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(3, result!.Number);
        Assert.Equal(new TimeSpan(12, 20, 0), result.StartTime);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99))
            .ReturnsAsync((TimeSlot?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WhenValidDto_ReturnsCreatedDto()
    {
        var dto = new CreateTimeSlotDto
        {
            Number = 4,
            StartTime = new TimeSpan(14, 15, 0),
            EndTime = new TimeSpan(15, 50, 0)
        };

        var created = new TimeSlot
        {
            Id = 10,
            Number = 4,
            StartTime = new TimeSpan(14, 15, 0),
            EndTime = new TimeSpan(15, 50, 0)
        };

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TimeSlot>()))
            .ReturnsAsync(created);

        var result = await _service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal(4, result.Number);
    }

    [Fact]
    public async Task UpdateAsync_WhenExists_UpdatesRepository()
    {
        var existing = new TimeSlot
        {
            Id = 5,
            Number = 1,
            StartTime = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(9, 0, 0)
        };

        var dto = new TimeSlotDto
        {
            Id = 5,
            Number = 2,
            StartTime = new TimeSpan(9, 30, 0),
            EndTime = new TimeSpan(11, 0, 0)
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(existing);

        await _service.UpdateAsync(dto);

        Assert.Equal(2, existing.Number);
        Assert.Equal(new TimeSpan(9, 30, 0), existing.StartTime);
        Assert.Equal(new TimeSpan(11, 0, 0), existing.EndTime);

        _repositoryMock.Verify(r => r.UpdateAsync(existing), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenNotExists_ThrowsException()
    {
        var dto = new TimeSlotDto
        {
            Id = 100,
            Number = 5,
            StartTime = new TimeSpan(16, 0, 0),
            EndTime = new TimeSpan(17, 30, 0)
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(100))
            .ReturnsAsync((TimeSlot?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(dto));
    }

    [Fact]
    public async Task DeleteAsync_WhenCalled_CallsRepository()
    {
        const int id = 7;

        await _service.DeleteAsync(id);

        _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
    }
}