namespace Planora.Domain.Entities;

public class Teacher : User
{

    public void GetTodaysSchedule() { throw new NotImplementedException(); }
    public void GetWeeklyWorkload() { throw new NotImplementedException(); }
    public void FindFreeClassrooms(TimeSpan startTime, TimeSpan endTime) { throw new NotImplementedException(); }
}
