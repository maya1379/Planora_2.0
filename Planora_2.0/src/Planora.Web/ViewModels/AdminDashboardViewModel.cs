using Planora.Domain.Entities;

namespace Planora.Web.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalGroups { get; set; }
    public int TotalBuildings { get; set; }
    public int TotalScheduleEntries { get; set; }
    public int TotalSubjects { get; set; }
    public List<User> RecentUsers { get; set; } = new();
}
