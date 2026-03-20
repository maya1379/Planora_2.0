namespace Planora.Domain.Entities;

public interface IScheduleProvider
{
    void GetTodaysSchedule();
    void GetWeeklySchedule();
}
