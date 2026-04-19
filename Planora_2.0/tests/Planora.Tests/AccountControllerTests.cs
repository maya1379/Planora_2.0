using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Planora.Domain.Entities;
using Planora.Infrastructure.Data;
using Planora.Services.Services.Interfaces;
using Planora.Web.Controllers;
using Planora.Web.ViewModels;
using Xunit;

namespace Planora.Tests;

public class AccountControllerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly PlanoraDbContext _context;
    private readonly Mock<IEmailService> _emailServiceMock;

    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<User>>();

        _signInManagerMock = new Mock<SignInManager<User>>(
            _userManagerMock.Object,
            contextAccessor.Object,
            userPrincipalFactory.Object,
            null!, null!, null!, null!);

        var options = new DbContextOptionsBuilder<PlanoraDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _context = new PlanoraDbContext(options);

        _emailServiceMock = new Mock<IEmailService>();

        _controller = new AccountController(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _context,
            _emailServiceMock.Object);
    }

    [Fact]
    public void Login_Get_ReturnsView()
    {
        var result = _controller.Login();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Login_Post_Success_Redirects()
    {
        var model = new LoginViewModel
        {
            Email = "test@test.com",
            Password = "123456",
            RememberMe = true
        };

        _signInManagerMock
            .Setup(x => x.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        var result = await _controller.Login(model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task Login_Post_Fail_ReturnsViewWithError()
    {
        var model = new LoginViewModel
        {
            Email = "test@test.com",
            Password = "wrong"
        };

        _signInManagerMock
            .Setup(x => x.PasswordSignInAsync(model.Email, model.Password, false, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        var result = await _controller.Login(model);

        var view = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Register_Post_Success_Redirects()
    {
        var model = new RegisterViewModel
        {
            Email = "test@test.com",
            Password = "123456",
            FullName = "Test User"
        };

        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<User>(), model.Password))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _controller.Register(model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public async Task Logout_Redirects()
    {
        var result = await _controller.Logout();

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }

    [Fact]
    public void AccessDenied_ReturnsView()
    {
        var result = _controller.AccessDenied();

        Assert.IsType<ViewResult>(result);
    }
}