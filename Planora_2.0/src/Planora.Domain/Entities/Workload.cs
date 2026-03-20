namespace Planora.Domain.Entities;

public class Workload
{
    public int Id { get; set; }

    public string? TeacherId { get; set; }
    public User? Teacher { get; set; }
    public int ScheduledHours { get; set; }

    public int CalculateScheduledHours() 
    { 
        if (Teacher?.TeachingAssignments != null)
        {
            ScheduledHours = Teacher.TeachingAssignments.Sum(ta => ta.HoursPerWeek);
        }
        return ScheduledHours; 
    }

    public int GetRemainingHours() 
    { 

        int maxAllowed = 40; 
        return maxAllowed - CalculateScheduledHours(); 
    }

    public void GenerateWorkload() 
    { 

        CalculateScheduledHours();
        if (ScheduledHours > 40)
        {
            throw new InvalidOperationException("Запланованих годин більше, ніж максимально дозволене навантаження (40 год).");
        }
    }
}
