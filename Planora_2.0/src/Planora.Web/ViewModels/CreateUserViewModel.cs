using System.ComponentModel.DataAnnotations;
using Planora.Domain.Enums;

namespace Planora.Web.ViewModels;

public class CreateUserViewModel
{
    [Required(ErrorMessage = "Введіть ПІБ")]
    [Display(Name = "Повне ім'я")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введіть email")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введіть пароль")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Роль")]
    public UserRole Role { get; set; } = UserRole.Teacher;

    [Display(Name = "Факультет")]
    public string? Faculty { get; set; }

    [Display(Name = "Посада")]
    public string? Position { get; set; }
}
