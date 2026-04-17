using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Planora.Domain.Constants;
using Planora.Web.ViewModels;
using Xunit;

namespace Planora.Tests;

public class UserViewModelsTests
{
    private static List<ValidationResult> ValidateModel(object model)
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }

    [Fact]
    public void CreateUserViewModel_WhenValid_ReturnsNoValidationErrors()
    {
        var model = new CreateUserViewModel
        {
            FullName = "Teacher User",
            Email = "teacher@example.com",
            Password = "123456",
            Role = AppRoles.Teacher,
            Faculty = "FIT",
            Position = "Assistant"
        };

        var results = ValidateModel(model);

        Assert.Empty(results);
    }

    [Fact]
    public void CreateUserViewModel_WhenFullNameMissing_ReturnsValidationError()
    {
        var model = new CreateUserViewModel
        {
            FullName = "",
            Email = "teacher@example.com",
            Password = "123456",
            Role = AppRoles.Teacher
        };

        var results = ValidateModel(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateUserViewModel.FullName)));
    }

    [Fact]
    public void CreateUserViewModel_WhenEmailMissing_ReturnsValidationError()
    {
        var model = new CreateUserViewModel
        {
            FullName = "Teacher User",
            Email = "",
            Password = "123456",
            Role = AppRoles.Teacher
        };

        var results = ValidateModel(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateUserViewModel.Email)));
    }

    [Fact]
    public void CreateUserViewModel_WhenEmailInvalid_ReturnsValidationError()
    {
        var model = new CreateUserViewModel
        {
            FullName = "Teacher User",
            Email = "invalid-email",
            Password = "123456",
            Role = AppRoles.Teacher
        };

        var results = ValidateModel(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateUserViewModel.Email)));
    }

    [Fact]
    public void CreateUserViewModel_WhenPasswordTooShort_ReturnsValidationError()
    {
        var model = new CreateUserViewModel
        {
            FullName = "Teacher User",
            Email = "teacher@example.com",
            Password = "123",
            Role = AppRoles.Teacher
        };

        var results = ValidateModel(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(CreateUserViewModel.Password)));
    }

    [Fact]
    public void CreateUserViewModel_DefaultRole_IsTeacher()
    {
        var model = new CreateUserViewModel();

        Assert.Equal(AppRoles.Teacher, model.Role);
    }

    [Fact]
    public void EditUserViewModel_WhenValid_ReturnsNoValidationErrors()
    {
        var model = new EditUserViewModel
        {
            Id = "user-1",
            FullName = "Edited User",
            Faculty = "FIT",
            Position = "Lecturer",
            GroupId = 1,
            Role = AppRoles.Teacher
        };

        var results = ValidateModel(model);

        Assert.Empty(results);
    }

    [Fact]
    public void EditUserViewModel_WhenFullNameMissing_ReturnsValidationError()
    {
        var model = new EditUserViewModel
        {
            Id = "user-1",
            FullName = "",
            Role = AppRoles.Teacher
        };

        var results = ValidateModel(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(EditUserViewModel.FullName)));
    }

    [Fact]
    public void EditUserViewModel_DefaultId_IsEmptyString()
    {
        var model = new EditUserViewModel();

        Assert.Equal(string.Empty, model.Id);
    }

    [Fact]
    public void AdminDashboardViewModel_RecentUsers_IsInitialized()
    {
        var model = new AdminDashboardViewModel();

        Assert.NotNull(model.RecentUsers);
        Assert.Empty(model.RecentUsers);
    }

    [Fact]
    public void AdminDashboardViewModel_Properties_CanBeAssigned()
    {
        var model = new AdminDashboardViewModel
        {
            TotalStudents = 100,
            TotalTeachers = 20,
            TotalGroups = 10,
            TotalBuildings = 3,
            TotalScheduleEntries = 250
        };

        Assert.Equal(100, model.TotalStudents);
        Assert.Equal(20, model.TotalTeachers);
        Assert.Equal(10, model.TotalGroups);
        Assert.Equal(3, model.TotalBuildings);
        Assert.Equal(250, model.TotalScheduleEntries);
    }
}