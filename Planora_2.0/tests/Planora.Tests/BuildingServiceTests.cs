namespace Planora.Tests;

using Xunit;
using Moq;
using AutoMapper;
using Planora.Services.Services;
using Planora.Services.DTOs;
using Planora.Domain.Entities;
using Planora.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

public class BuildingServiceTests
{
    private readonly Mock<IBuildingRepository> _repoMock;
    private readonly IMapper _mapper;
    private readonly BuildingService _service;

    public BuildingServiceTests()
    {
        _repoMock = new Mock<IBuildingRepository>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Building, BuildingDto>()
                .ForMember(dest => dest.ClassroomCount, opt => opt.MapFrom(src => src.Classrooms.Count));

            cfg.CreateMap<CreateBuildingDto, Building>();
            cfg.CreateMap<UpdateBuildingDto, Building>();
        });

        _mapper = config.CreateMapper();
        _service = new BuildingService(_repoMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsBuildings()
    {
        var buildings = new List<Building>
        {
            new Building { Id = 1, Name = "A", Classrooms = new List<Classrooms>() }
        };

        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(buildings);

        var result = await _service.GetAllAsync();

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsBuilding_WhenExists()
    {
        var building = new Building { Id = 1, Name = "A", Classrooms = new List<Classrooms>() };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(building);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("A", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Building?)null);

        var result = await _service.GetByIdAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesBuilding()
    {
        var dto = new CreateBuildingDto { Name = "New" };
        var building = new Building { Id = 1, Name = "New", Classrooms = new List<Classrooms>() };

        _repoMock.Setup(r => r.AddAsync(It.IsAny<Building>())).ReturnsAsync(building);

        var result = await _service.CreateAsync(dto);

        Assert.Equal("New", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_Throws_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Building?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.UpdateAsync(new UpdateBuildingDto { Id = 1 }));
    }

    [Fact]
    public async Task DeleteAsync_CallsRepository()
    {
        await _service.DeleteAsync(1);

        _repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}