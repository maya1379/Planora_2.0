namespace Planora.Domain.Entities;

public class Building
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Address { get; set; }

    public virtual ICollection<Classrooms> Classrooms { get; set; } = new List<Classrooms>();
}
