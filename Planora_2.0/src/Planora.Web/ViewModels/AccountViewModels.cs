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
    [Required(ErrorMessage = "Введіть ПІБ")]
    [Display(Name = "Повне ім'я")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введіть email")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введіть пароль")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль повинен бути не менше 6 символів")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Підтвердіть пароль")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Паролі не співпадають")]
    [Display(Name = "Підтвердження паролю")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Роль")]
    public UserRole Role { get; set; }

    [Display(Name = "Факультет")]
    public string? Faculty { get; set; }
}
