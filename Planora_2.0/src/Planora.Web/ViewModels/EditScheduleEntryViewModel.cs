using System.ComponentModel.DataAnnotations;
using Planora.Domain.Enums;

namespace Planora.Web.ViewModels;

public class EditScheduleEntryViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Оберіть день тижня")]
    [Display(Name = "День тижня")]
    public DayOfWeekEnum DayOfWeek { get; set; }

    [Required(ErrorMessage = "Оберіть часовий слот")]
    [Display(Name = "Пара")]
    public int TimeSlotId { get; set; }

    [Required(ErrorMessage = "Оберіть аудиторію")]
    [Display(Name = "Аудиторія")]
    public int ClassroomId { get; set; }

    [Required(ErrorMessage = "Оберіть викладача")]
    [Display(Name = "Викладач")]
    public string TeacherId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Оберіть предмет")]
    [Display(Name = "Предмет")]
    public int SubjectId { get; set; }

    [Required(ErrorMessage = "Оберіть групу")]
    [Display(Name = "Група")]
    public int GroupId { get; set; }

    [Display(Name = "Тип тижня")]
    public WeekType WeekType { get; set; }
}
