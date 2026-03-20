namespace Planora.Domain.Entities;

public class Student : User
{

    public void GetTodaysSchedule() { throw new NotImplementedException(); }
    public void GetWeeklySchedule() { throw new NotImplementedException(); }
    public void FindFreeClassrooms(TimeSpan startTime, TimeSpan endTime) { throw new NotImplementedException(); }
}
