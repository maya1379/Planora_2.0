using System.ComponentModel.DataAnnotations;
using Planora.Domain.Enums;

namespace Planora.Web.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Введіть email")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введіть пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Запам'ятати мене")]
    public bool RememberMe { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "ПІБ є обов'язковим")]
    [Display(Name = "Повне ім'я")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email є обов'язковим")]
    [EmailAddress(ErrorMessage = "Неправильний формат Email")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль є обов'язковим")]
    [StringLength(100, ErrorMessage = "{0} має бути мінімум {2} символів.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Підтвердження паролю")]
    [Compare("Password", ErrorMessage = "Паролі не збігаються.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Роль є обов'язковою")]
    [Display(Name = "Роль")]
    public UserRole Role { get; set; }

    [Display(Name = "Факультет")]
    public string? Faculty { get; set; }

    [Display(Name = "Посада (для викладачів)")]
    public string? Position { get; set; }

    [Display(Name = "Група (для студентів)")]
    public int? GroupId { get; set; }
}

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Email є обов'язковим")]
    [EmailAddress(ErrorMessage = "Неправильний формат Email")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordViewModel
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль є обов'язковим")]
    [StringLength(100, ErrorMessage = "{0} має бути мінімум {2} символів.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Новий пароль")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Підтвердження паролю")]
    [Compare("Password", ErrorMessage = "Паролі не збігаються.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
