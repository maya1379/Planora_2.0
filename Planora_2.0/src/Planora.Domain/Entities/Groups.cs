namespace Planora.Domain.Entities;

public class Groups
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Faculty { get; set; } = string.Empty;

    public int StudentCount { get; set; }
    public string StarostaName { get; set; } = string.Empty;

    public virtual ICollection<GroupDisciplineList> GroupDisciplineLists { get; set; } = new List<GroupDisciplineList>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
