using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planora.Domain.Entities;
using Planora.Domain.Enums;
using Planora.Infrastructure.Data;
using Planora.Web.Controllers;
using Xunit;

namespace Planora.Tests;

public class SearchControllerTests
{
    private static PlanoraDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<PlanoraDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new PlanoraDbContext(options);
    }

    [Fact]
    public async Task TeacherSearch_WithoutFilter_ReturnsAllTeachersOrdered()
    {
        using var context = CreateContext(nameof(TeacherSearch_WithoutFilter_ReturnsAllTeachersOrdered));

        context.Users.AddRange(
            new User
            {
                Id = "t2",
                FullName = "Petrenko Ivan",
                Role = UserRole.Teacher
            },
            new User
            {
                Id = "t1",
                FullName = "Antonenko Oleg",
                Role = UserRole.Teacher
            },
            new User
            {
                Id = "s1",
                FullName = "Student User",
                Role = UserRole.Student
            });

        await context.SaveChangesAsync();

        var controller = new SearchController(context);

        var result = await controller.TeacherSearch(null);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<User>>(viewResult.Model).ToList();

        Assert.Equal(2, model.Count);
        Assert.Equal("Antonenko Oleg", model[0].FullName);
        Assert.Equal("Petrenko Ivan", model[1].FullName);
        Assert.Null(controller.ViewBag.SearchTerm);
    }

    [Fact]
    public async Task TeacherSearch_WithFilter_ReturnsMatchingTeachers()
    {
        using var context = CreateContext(nameof(TeacherSearch_WithFilter_ReturnsMatchingTeachers));

        context.Users.AddRange(
            new User
            {
                Id = "t1",
                FullName = "Ivan Ivanov",
                Role = UserRole.Teacher
            },
            new User
            {
                Id = "t2",
                FullName = "Petro Petrenko",
                Role = UserRole.Teacher
            },
            new User
            {
                Id = "s1",
                FullName = "Ivan Student",
                Role = UserRole.Student
            });

        await context.SaveChangesAsync();

        var controller = new SearchController(context);

        var result = await controller.TeacherSearch("Ivan");

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<User>>(viewResult.Model).ToList();

        Assert.Single(model);
        Assert.Equal("Ivan Ivanov", model[0].FullName);
        Assert.Equal("Ivan", controller.ViewBag.SearchTerm);
    }

    [Fact]
    public async Task TeacherSearch_WithNoMatches_ReturnsEmptyList()
    {
        using var context = CreateContext(nameof(TeacherSearch_WithNoMatches_ReturnsEmptyList));

        context.Users.AddRange(
            new User
            {
                Id = "t1",
                FullName = "Ivan Ivanov",
                Role = UserRole.Teacher
            },
            new User
            {
                Id = "t2",
                FullName = "Petro Petrenko",
                Role = UserRole.Teacher
            });

        await context.SaveChangesAsync();

        var controller = new SearchController(context);

        var result = await controller.TeacherSearch("XYZ");

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<User>>(viewResult.Model);

        Assert.Empty(model);
        Assert.Equal("XYZ", controller.ViewBag.SearchTerm);
    }

    [Fact]
    public async Task ClassroomAvailability_WhenNoParams_UsesDefaultsAndReturnsAllClassrooms()
    {
        using var context = CreateContext(nameof(ClassroomAvailability_WhenNoParams_UsesDefaultsAndReturnsAllClassrooms));

        var building = new Building
        {
            Id = 1,
            Name = "Main Building"
        };

        context.Buildings.Add(building);

        context.Classrooms.AddRange(
            new Classrooms
            {
                Id = 1,
                Number = "101",
                BuildingId = 1
            },
            new Classrooms
            {
                Id = 2,
                Number = "102",
                BuildingId = 1
            });

        context.TimeSlots.AddRange(
            new TimeSlot
            {
                Id = 10,
                Number = 1,
                StartTime = new TimeSpan(8, 30, 0),
                EndTime = new TimeSpan(10, 5, 0)
            },
            new TimeSlot
            {
                Id = 20,
                Number = 2,
                StartTime = new TimeSpan(10, 25, 0),
                EndTime = new TimeSpan(12, 0, 0)
            });

        await context.SaveChangesAsync();

        var controller = new SearchController(context);

        var result = await controller.ClassroomAvailability(null, null);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Classrooms>>(viewResult.Model).ToList();

        Assert.Equal(2, model.Count);
        Assert.Equal(DayOfWeekEnum.Monday, controller.ViewBag.SelectedDay);
        Assert.Equal(10, controller.ViewBag.SelectedTimeSlotId);
        Assert.NotNull(controller.ViewBag.Days);
        Assert.NotNull(controller.ViewBag.TimeSlots);
        Assert.NotNull(controller.ViewBag.OccupiedClassroomsIds);
    }

    [Fact]
    public async Task ClassroomAvailability_WithParams_SetsSelectedValuesAndOccupiedIds()
    {
        using var context = CreateContext(nameof(ClassroomAvailability_WithParams_SetsSelectedValuesAndOccupiedIds));

        var building = new Building
        {
            Id = 1,
            Name = "Main Building"
        };

        context.Buildings.Add(building);

        context.Classrooms.AddRange(
            new Classrooms
            {
                Id = 1,
                Number = "101",
                BuildingId = 1
            },
            new Classrooms
            {
                Id = 2,
                Number = "102",
                BuildingId = 1
            });

        context.TimeSlots.AddRange(
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
            });

        context.Schedules.Add(
            new Schedule
            {
                Id = 1,
                DayOfWeek = DayOfWeekEnum.Wednesday,
                TimeSlotId = 2,
                ClassroomId = 2
            });

        await context.SaveChangesAsync();

        var controller = new SearchController(context);

        var result = await controller.ClassroomAvailability(DayOfWeekEnum.Wednesday, 2);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Classrooms>>(viewResult.Model).ToList();

        Assert.Equal(2, model.Count);
        Assert.Equal(DayOfWeekEnum.Wednesday, controller.ViewBag.SelectedDay);
        Assert.Equal(2, controller.ViewBag.SelectedTimeSlotId);

        var occupied = Assert.IsAssignableFrom<IEnumerable<int>>(controller.ViewBag.OccupiedClassroomsIds);
        Assert.Contains(2, occupied);
    }

    [Fact]
    public async Task ClassroomAvailability_WhenNoTimeSlots_UsesFallbackTimeSlotId1()
    {
        using var context = CreateContext(nameof(ClassroomAvailability_WhenNoTimeSlots_UsesFallbackTimeSlotId1));

        var building = new Building
        {
            Id = 1,
            Name = "Main Building"
        };

        context.Buildings.Add(building);

        context.Classrooms.Add(
            new Classrooms
            {
                Id = 1,
                Number = "101",
                BuildingId = 1
            });

        await context.SaveChangesAsync();

        var controller = new SearchController(context);

        var result = await controller.ClassroomAvailability(null, null);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Classrooms>>(viewResult.Model).ToList();

        Assert.Single(model);
        Assert.Equal(DayOfWeekEnum.Monday, controller.ViewBag.SelectedDay);
        Assert.Equal(1, controller.ViewBag.SelectedTimeSlotId);
    }
}