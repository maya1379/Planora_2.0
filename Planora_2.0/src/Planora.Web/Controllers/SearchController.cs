using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planora.Domain.Enums;
using Planora.Infrastructure.Data;

namespace Planora.Web.Controllers;

public class SearchController : Controller
{
    private readonly PlanoraDbContext _context;

    public SearchController(PlanoraDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> TeacherSearch(string? teacherName)
    {
        var query = _context.Users
            .Where(u => u.Role == UserRole.Teacher)
            .Include(u => u.TeachingAssignments)
                .ThenInclude(ta => ta.Subjects)
            .Include(u => u.Schedules)
                .ThenInclude(s => s.Classrooms)
            .Include(u => u.Schedules)
                .ThenInclude(s => s.TimeSlot)
            .Include(u => u.Schedules)
                .ThenInclude(s => s.Subjects)
            .Include(u => u.Schedules)
                .ThenInclude(s => s.Groups)
            .AsQueryable();

        if (!string.IsNullOrEmpty(teacherName))
        {
            query = query.Where(u => u.FullName.Contains(teacherName));
        }

        var teachers = await query.OrderBy(u => u.FullName).ToListAsync();
        ViewBag.SearchTerm = teacherName;

        return View(teachers);
    }

    public async Task<IActionResult> TeacherDetails(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var teacher = await _context.Users
            .Include(u => u.TeachingAssignments)
                .ThenInclude(ta => ta.Subjects)
            .Include(u => u.Schedules)
                .ThenInclude(s => s.Classrooms)
            .Include(u => u.Schedules)
                .ThenInclude(s => s.TimeSlot)
            .Include(u => u.Schedules)
                .ThenInclude(s => s.Subjects)
            .Include(u => u.Schedules)
                .ThenInclude(s => s.Groups)
            .FirstOrDefaultAsync(u => u.Id == id && u.Role == UserRole.Teacher);

        if (teacher == null)
            return NotFound();

        return View(teacher);
    }

    public async Task<IActionResult> ClassroomAvailability(DayOfWeekEnum? day, int? timeSlotId)
    {
        var classrooms = await _context.Classrooms
            .Include(c => c.Building)
            .ToListAsync();

        var timeSlots = await _context.TimeSlots.OrderBy(ts => ts.Number).ToListAsync();

        var selectedDay = day ?? DayOfWeekEnum.Monday;
        var selectedTimeSlotId = timeSlotId ?? (timeSlots.FirstOrDefault()?.Id ?? 1);

        var occupiedClassroomsIds = await _context.Schedules
            .Where(s => s.DayOfWeek == selectedDay && s.TimeSlotId == selectedTimeSlotId)
            .Select(s => s.ClassroomId)
            .ToListAsync();

        ViewBag.Days = Enum.GetValues<DayOfWeekEnum>().Where(d => d >= DayOfWeekEnum.Monday && d <= DayOfWeekEnum.Friday).ToList();
        ViewBag.TimeSlots = timeSlots;
        ViewBag.SelectedDay = selectedDay;
        ViewBag.SelectedTimeSlotId = selectedTimeSlotId;
        ViewBag.OccupiedClassroomsIds = occupiedClassroomsIds;

        return View(classrooms);
    }
}
