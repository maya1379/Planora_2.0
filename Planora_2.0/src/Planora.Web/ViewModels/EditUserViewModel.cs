using System.ComponentModel.DataAnnotations;
using Planora.Domain.Constants;

namespace Planora.Web.ViewModels;

public class EditUserViewModel
{
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введіть ПІБ")]
    [Display(Name = "Повне ім'я")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Факультет")]
    public string? Faculty { get; set; }

    [Display(Name = "Посада")]
    public string? Position { get; set; }

    [Display(Name = "Група")]
    public int? GroupId { get; set; }

    public string Role { get; set; } = string.Empty;
}
