namespace Planora.Domain.Entities;

public class Classrooms
{
    public int Id { get; set; }

    public string Number { get; set; } = string.Empty;

    public int Capacity { get; set; }

    public bool HasComputers { get; set; }

    public bool HasProjector { get; set; }

    public string Faculty { get; set; } = string.Empty;

    public int BuildingId { get; set; }

    public virtual Building Building { get; set; } = null!;

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
