using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Planora.Domain.Constants;
using Planora.Web.ViewModels;
using Xunit;

namespace Planora.Tests;

public class AccountViewModelsTests
{
    private static List<ValidationResult> ValidateModel(object model)
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    [Fact]
    public void LoginViewModel_WhenValid_ReturnsNoValidationErrors()
    {
        var model = new LoginViewModel
        {
            Email = "test@example.com",
            Password = "123456",
            RememberMe = true
        };

        var results = ValidateModel(model);

        Assert.Empty(results);
    }

    [Fact]
    public void LoginViewModel_WhenEmailIsMissing_ReturnsValidationError()
    {
        var model = new LoginViewModel
        {
            Email = "",
            Password = "123456"
        };

        var results = ValidateModel(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(LoginViewModel.Email)));
    }

    [Fact]
    public void LoginViewModel_WhenPasswordIsMissing_ReturnsValidationError()
    {
        var model = new LoginViewModel
        {
            Email = "test@example.com",
            Password = ""
        };

        var results = ValidateModel(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(LoginViewModel.Password)));
    }

    [Fact]
    public void LoginViewModel_WhenEmailFormatIsInvalid_ReturnsValidationError()
    {
        var model = new LoginViewModel
        {
            Email = "invalid-email",
            Password = "123456"
        };

        var results = ValidateModel(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(LoginViewModel.Email)));
    }

    [Fact]
    public void RegisterViewModel_WhenValid_ReturnsNoValidationErrors()
    {
        var model = new RegisterViewModel
        {
            FullName = "Test User",
            Email = "test@example.com",
            Password = "123456",
            ConfirmPassword = "123456",
            Role = AppRoles.Teacher,
            Faculty = "FIT",
            Position = "Professor"
        };

        var results = ValidateModel(model);

        Assert.Empty(results);
    }

    [Fact]
    public void RegisterViewModel_WhenFullNameIsMissing_ReturnsValidationError()
    {
        var model = new RegisterViewModel
        {
            FullName = "",
            Email = "test@example.com",
            Password = "123456",
            ConfirmPassword = "123456",
            Role = AppRoles.Teacher
        };

        var results = ValidateModel(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RegisterViewModel.FullName)));
    }

    [Fact]
    public void RegisterViewModel_WhenEmailIsInvalid_ReturnsValidationError()
    {
        var model = new RegisterViewModel
        {
            FullName = "Test User",
            Email = "wrong-email",
            Password = "123456",
            ConfirmPassword = "123456",
            Role = AppRoles.Teacher
        };

        var results = ValidateModel(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RegisterViewModel.Email)));
    }

    [Fact]
    public void RegisterViewModel_WhenPasswordTooShort_ReturnsValidationError()
    {
        var model = new RegisterViewModel
        {
            FullName = "Test User",
            Email = "test@example.com",
            Password = "123",
            ConfirmPassword = "123",
            Role = AppRoles.Teacher
        };

        var results = ValidateModel(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RegisterViewModel.Password)));
    }

    [Fact]
    public void RegisterViewModel_WhenPasswordsDoNotMatch_ReturnsValidationError()
    {
        var model = new RegisterViewModel
        {
            FullName = "Test User",
            Email = "test@example.com",
            Password = "123456",
            ConfirmPassword = "654321",
            Role = AppRoles.Teacher
        };

        var results = ValidateModel(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RegisterViewModel.ConfirmPassword)));
    }

    [Fact]
    public void RegisterViewModel_WhenOptionalFieldsAreNull_StillValid()
    {
        var model = new RegisterViewModel
        {
            FullName = "Student User",
            Email = "student@example.com",
            Password = "123456",
            ConfirmPassword = "123456",
            Role = AppRoles.Student,
            Faculty = null,
            Position = null,
            GroupId = null
        };

        var results = ValidateModel(model);

        Assert.Empty(results);
    }
}