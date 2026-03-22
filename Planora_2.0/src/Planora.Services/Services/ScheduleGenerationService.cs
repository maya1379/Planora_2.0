using AutoMapper;
using Planora.Services.DTOs;
using Planora.Services.Interfaces;
using Planora.Services.Services.Interfaces;
using Planora.Domain.Entities;
using Planora.Domain.Enums;

namespace Planora.Services.Services;

public class ScheduleGenerationService : IScheduleGenerationService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupSubjectRepository _groupSubjectRepository;
    private readonly ITeachingAssignmentRepository _teachingAssignmentRepository;
    private readonly IClassroomRepository _classroomRepository;
    private readonly ITimeSlotRepository _timeSlotRepository;
    private readonly IScheduleEntryRepository _scheduleEntryRepository;
    private readonly IMapper _mapper;

    public ScheduleGenerationService(
        IGroupRepository groupRepository,
        IGroupSubjectRepository groupSubjectRepository,
        ITeachingAssignmentRepository teachingAssignmentRepository,
        IClassroomRepository classroomRepository,
        ITimeSlotRepository timeSlotRepository,
        IScheduleEntryRepository scheduleEntryRepository,
        IMapper mapper)
    {
        _groupRepository = groupRepository;
        _groupSubjectRepository = groupSubjectRepository;
        _teachingAssignmentRepository = teachingAssignmentRepository;
        _classroomRepository = classroomRepository;
        _timeSlotRepository = timeSlotRepository;
        _scheduleEntryRepository = scheduleEntryRepository;
        _mapper = mapper;
    }

    public async Task<ScheduleGenerationResultDto> GenerateScheduleAsync()
    {
        var result = new ScheduleGenerationResultDto();

        try
        {
            var groups = (await _groupRepository.GetAllAsync()).ToList();
            var teachingAssignments = (await _teachingAssignmentRepository.GetAllAsync()).ToList();
            var classrooms = (await _classroomRepository.GetAllAsync()).ToList();
            var timeSlots = (await _timeSlotRepository.GetAllAsync()).OrderBy(ts => ts.Number).ToList();

            if (!groups.Any())
            {
                result.Errors.Add("Немає жодної групи в системі.");
                return result;
            }

            if (!teachingAssignments.Any())
            {
                result.Errors.Add("Немає навчального навантаження викладачів.");
                return result;
            }

            if (!classrooms.Any())
            {
                result.Errors.Add("Немає жодної аудиторії в системі.");
                return result;
            }

            if (!timeSlots.Any())
            {
                result.Errors.Add("Немає тайм-слотів (пар) в системі.");
                return result;
            }

            await _scheduleEntryRepository.DeleteAllAsync();

            var scheduleRequirements = BuildScheduleRequirements(groups, teachingAssignments);

            if (!scheduleRequirements.Any())
            {
                result.Errors.Add("Не вдалося побудувати вимоги до розкладу. Перевірте навантаження викладачів.");
                return result;
            }

            var days = Enum.GetValues<DayOfWeekEnum>()
                           .Where(d => d >= DayOfWeekEnum.Monday && d <= DayOfWeekEnum.Friday)
                           .ToList();

            var sortedRequirements = scheduleRequirements
                .OrderByDescending(r => r.HoursPerWeek)
                .ToList();

            var placedEntries = new List<Schedule>();
            var occupiedSlots = new HashSet<string>(); 

            foreach (var requirement in sortedRequirements)
            {
                int hoursToPlace = requirement.HoursPerWeek;

                var weekTypeForPlacement = hoursToPlace >= 2
                    ? WeekType.Both
                    : WeekType.Numerator; 

                var orderedDays = days.OrderBy(d => (int)d).ToList();

                foreach (var timeSlot in timeSlots)
                {
                    if (hoursToPlace <= 0) break;

                    foreach (var day in orderedDays)
                    {
                        if (hoursToPlace <= 0) break;

                        if (!IsConsecutiveSlot(placedEntries, requirement.GroupId, day, timeSlot, timeSlots, weekTypeForPlacement))
                            continue;

                        var dailyClassesCount = placedEntries.Count(e => e.GroupId == requirement.GroupId && e.DayOfWeek == day &&
                            (e.WeekType == WeekType.Both || e.WeekType == weekTypeForPlacement || weekTypeForPlacement == WeekType.Both));
                        if (dailyClassesCount >= 4)
                            continue;

                        string teacherSlotKey = $"T:{requirement.TeacherId}|D:{day}|TS:{timeSlot.Id}|W:{weekTypeForPlacement}";
                        if (occupiedSlots.Contains(teacherSlotKey)) continue;

                        string groupSlotKey = $"G:{requirement.GroupId}|D:{day}|TS:{timeSlot.Id}|W:{weekTypeForPlacement}";
                        if (occupiedSlots.Contains(groupSlotKey)) continue;

                        if (weekTypeForPlacement != WeekType.Both)
                        {
                            string teacherBothKey = $"T:{requirement.TeacherId}|D:{day}|TS:{timeSlot.Id}|W:{WeekType.Both}";
                            string groupBothKey = $"G:{requirement.GroupId}|D:{day}|TS:{timeSlot.Id}|W:{WeekType.Both}";
                            if (occupiedSlots.Contains(teacherBothKey) || occupiedSlots.Contains(groupBothKey)) continue;
                        }
                        else
                        {
                            string teacherNumKey = $"T:{requirement.TeacherId}|D:{day}|TS:{timeSlot.Id}|W:{WeekType.Numerator}";
                            string teacherDenKey = $"T:{requirement.TeacherId}|D:{day}|TS:{timeSlot.Id}|W:{WeekType.Denominator}";
                            string groupNumKey = $"G:{requirement.GroupId}|D:{day}|TS:{timeSlot.Id}|W:{WeekType.Numerator}";
                            string groupDenKey = $"G:{requirement.GroupId}|D:{day}|TS:{timeSlot.Id}|W:{WeekType.Denominator}";
                            if (occupiedSlots.Contains(teacherNumKey) || occupiedSlots.Contains(teacherDenKey) ||
                                occupiedSlots.Contains(groupNumKey) || occupiedSlots.Contains(groupDenKey)) continue;
                        }

                        var suitableClassroom = FindSuitableClassroom(
                            classrooms,
                            requirement.GroupStudentCount,
                            requirement.LessonType,
                            day,
                            timeSlot.Id,
                            weekTypeForPlacement,
                            occupiedSlots,
                            placedEntries,
                            requirement.GroupId);

                        if (suitableClassroom == null)
                        {
                            continue; 
                        }

                        var entry = new Schedule
                        {
                            DayOfWeek = day,
                            WeekType = weekTypeForPlacement,
                            TimeSlotId = timeSlot.Id,
                            ClassroomId = suitableClassroom.Id,
                            TeacherId = requirement.TeacherId,
                            SubjectId = requirement.SubjectId,
                            GroupId = requirement.GroupId
                        };

                        placedEntries.Add(entry);

                        occupiedSlots.Add(teacherSlotKey);
                        occupiedSlots.Add(groupSlotKey);
                        string classroomSlotKey = $"C:{suitableClassroom.Id}|D:{day}|TS:{timeSlot.Id}|W:{weekTypeForPlacement}";
                        occupiedSlots.Add(classroomSlotKey);

                        hoursToPlace -= (weekTypeForPlacement == WeekType.Both ? 2 : 1);
                    }
                }

                if (hoursToPlace > 0)
                {
                    result.Warnings.Add(
                        $"Не вдалося повністю розставити предмет '{requirement.SubjectName}' " +
                        $"для групи '{requirement.GroupName}' (не вистачає {hoursToPlace} год.)");
                }
            }

            if (placedEntries.Any())
            {
                await _scheduleEntryRepository.AddRangeAsync(placedEntries);
            }

            result.Success = !result.Errors.Any();
            result.TotalEntriesCreated = placedEntries.Count;

            if (result.Warnings.Any())
            {
                result.Success = true; 
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Помилка під час генерації: {ex.Message}");
            result.Success = false;
        }

        return result;
    }

    private List<ScheduleRequirement> BuildScheduleRequirements(
        List<Groups> groups,
        List<TeachingAssignment> teachingAssignments)
    {
        var requirements = new List<ScheduleRequirement>();

        foreach (var ta in teachingAssignments)
        {
            var group = groups.FirstOrDefault(g => g.Id == ta.GroupId);
            if (group == null) continue;

            requirements.Add(new ScheduleRequirement
            {
                GroupId = group.Id,
                GroupName = group.Name,
                GroupStudentCount = group.StudentCount,
                SubjectId = ta.SubjectId,
                SubjectName = ta.Subjects?.Name ?? "Unknown",
                LessonType = ta.Subjects?.Type ?? LessonType.Lecture,
                TeacherId = ta.TeacherId,
                TeacherName = ta.Teacher?.FullName ?? "Unknown",
                HoursPerWeek = ta.HoursPerWeek,
                TotalHours = ta.HoursPerWeek
            });
        }

        return requirements;
    }

    private Classrooms? FindSuitableClassroom(
        List<Classrooms> allClassrooms,
        int groupStudentCount,
        LessonType lessonType,
        DayOfWeekEnum day,
        int timeSlotId,
        WeekType weekType,
        HashSet<string> occupiedSlots,
        List<Schedule> placedEntries,
        int groupId)
    {
        var suitable = allClassrooms
            .Where(c => c.Capacity >= groupStudentCount)
            .ToList();

        if (lessonType == LessonType.Lab)
        {
            suitable = suitable.Where(c => c.HasComputers).ToList();
        }

        suitable = suitable.Where(c =>
        {
            string classroomSlotKey = $"C:{c.Id}|D:{day}|TS:{timeSlotId}|W:{weekType}";
            if (occupiedSlots.Contains(classroomSlotKey)) return false;

            if (weekType == WeekType.Both)
            {
                string numKey = $"C:{c.Id}|D:{day}|TS:{timeSlotId}|W:{WeekType.Numerator}";
                string denKey = $"C:{c.Id}|D:{day}|TS:{timeSlotId}|W:{WeekType.Denominator}";
                if (occupiedSlots.Contains(numKey) || occupiedSlots.Contains(denKey)) return false;
            }
            else
            {
                string bothKey = $"C:{c.Id}|D:{day}|TS:{timeSlotId}|W:{WeekType.Both}";
                if (occupiedSlots.Contains(bothKey)) return false;
            }

            return true;
        }).ToList();

        if (!suitable.Any()) return null;

        var groupEntriesForDay = placedEntries
            .Where(e => e.GroupId == groupId && e.DayOfWeek == day)
            .ToList();

        if (groupEntriesForDay.Any())
        {
            var usedBuildingId = allClassrooms
                .FirstOrDefault(c => c.Id == groupEntriesForDay.First().ClassroomId)?.BuildingId;

            if (usedBuildingId.HasValue)
            {
                var sameBuildingRooms = suitable.Where(c => c.BuildingId == usedBuildingId.Value).ToList();
                if (sameBuildingRooms.Any())
                {
                    return sameBuildingRooms.OrderBy(c => c.Capacity).First();
                }
            }
        }

        return suitable.OrderBy(c => c.Capacity).First();
    }

    private class ScheduleRequirement
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public int GroupStudentCount { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public LessonType LessonType { get; set; }
        public string TeacherId { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public int HoursPerWeek { get; set; }
        public int TotalHours { get; set; } 
    }

    private static bool IsConsecutiveSlot(
        List<Schedule> placedEntries,
        int groupId,
        DayOfWeekEnum day,
        TimeSlot candidateSlot,
        List<TimeSlot> allTimeSlots,
        WeekType weekType)
    {
        var groupEntriesForDay = placedEntries
            .Where(e => e.GroupId == groupId && e.DayOfWeek == day &&
                        (e.WeekType == WeekType.Both || e.WeekType == weekType ||
                         weekType == WeekType.Both))
            .ToList();

        if (!groupEntriesForDay.Any())
            return true;

        var occupiedSlotNumbers = groupEntriesForDay
            .Select(e => allTimeSlots.FirstOrDefault(ts => ts.Id == e.TimeSlotId)?.Number ?? 0)
            .OrderBy(n => n)
            .ToList();

        int candidateNumber = candidateSlot.Number;
        int min = occupiedSlotNumbers.Min();
        int max = occupiedSlotNumbers.Max();

        return candidateNumber == min - 1 || candidateNumber == max + 1;
    }
}
