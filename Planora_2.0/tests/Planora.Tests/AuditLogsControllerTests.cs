using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Planora.Domain.Entities;
using Planora.Infrastructure.Data;
using Planora.Web.Controllers;
using Planora.Web.ViewModels;
using System.Text.Json;
using Xunit;
using Planora.Tests.Helpers;

namespace Planora.Tests;

public class AuditLogsControllerTests : IDisposable
{
    private readonly PlanoraDbContext _context;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly AuditLogsController _controller;

    public AuditLogsControllerTests()
    {
        var options = new DbContextOptionsBuilder<PlanoraDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PlanoraDbContext(options);

        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var usersData = new List<User>
        {
            new User { Id = "user1", FullName = "Test User" }
        }.AsQueryable();

        var mockUsers = new Mock<IQueryable<User>>();
        mockUsers.As<IAsyncEnumerable<User>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestAsyncEnumerator<User>(usersData.GetEnumerator()));
        mockUsers.As<IQueryable<User>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<User>(usersData.Provider));
        mockUsers.As<IQueryable<User>>().Setup(m => m.Expression).Returns(usersData.Expression);
        mockUsers.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(usersData.ElementType);
        mockUsers.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(usersData.GetEnumerator());

        _userManagerMock.Setup(x => x.Users).Returns(mockUsers.Object);

        _controller = new AuditLogsController(_context, _userManagerMock.Object);
    }

    [Fact]
    public async Task Index_FiltersOutCreateLogs_AndMapsUserFullName()
    {
        _context.AuditLogs.AddRange(
            new AuditLog { Id = 1, Type = "Update", TableName = "Schedule", UserId = "user1", DateTime = DateTime.Now, PrimaryKey = "{}" },
            new AuditLog { Id = 2, Type = "Create", TableName = "Schedule", UserId = "user1", DateTime = DateTime.Now, PrimaryKey = "{}" },
            new AuditLog { Id = 3, Type = "Delete", TableName = "Schedule", UserId = "user1", DateTime = DateTime.Now, PrimaryKey = "{}" }
        );
        await _context.SaveChangesAsync();

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<AuditLogViewModel>>(viewResult.Model);

        Assert.Equal(2, model.Count());
        Assert.DoesNotContain(model, m => m.Type == "Create");
        Assert.All(model, m => Assert.Equal("Test User", m.UserName));
    }

    [Fact]
    public async Task Undo_Update_RevertsProperties()
    {
        var schedule = new Schedule { Id = 1, DayOfWeek = Domain.Enums.DayOfWeekEnum.Monday };
        _context.Schedules.Add(schedule);
        await _context.SaveChangesAsync();

        var oldValues = new Dictionary<string, int> { { "DayOfWeek", (int)Domain.Enums.DayOfWeekEnum.Friday } };

        var auditLog = new AuditLog
        {
            Id = 100, 
            Type = "Update",
            TableName = "Schedule",
            PrimaryKey = JsonSerializer.Serialize(new { Id = 1 }),
            OldValues = JsonSerializer.Serialize(oldValues),
            DateTime = DateTime.Now
        };
        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();

        var result = await _controller.Undo(100);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);

        var updatedSchedule = await _context.Schedules.FindAsync(1);
        Assert.NotNull(updatedSchedule);
        Assert.Equal(Domain.Enums.DayOfWeekEnum.Friday, updatedSchedule.DayOfWeek);
    }

    [Fact]
    public async Task Undo_Delete_RestoresRecord()
    {
        var oldValues = new Dictionary<string, object> 
        { 
            { "Id", 10 }, 
            { "DayOfWeek", (int)Domain.Enums.DayOfWeekEnum.Wednesday }
        };

        var auditLog = new AuditLog
        {
            Id = 101,
            Type = "Delete",
            TableName = "Schedule",
            PrimaryKey = JsonSerializer.Serialize(new { Id = 10 }),
            OldValues = JsonSerializer.Serialize(oldValues),
            DateTime = DateTime.Now
        };
        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();

        var result = await _controller.Undo(101);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);

        var restoredSchedule = await _context.Schedules.FindAsync(10);
        Assert.NotNull(restoredSchedule);
        Assert.Equal(Domain.Enums.DayOfWeekEnum.Wednesday, restoredSchedule.DayOfWeek);
    }

    [Fact]
    public async Task Undo_NotFound_ReturnsNotFound()
    {
        var result = await _controller.Undo(999);

        Assert.IsType<NotFoundResult>(result);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
