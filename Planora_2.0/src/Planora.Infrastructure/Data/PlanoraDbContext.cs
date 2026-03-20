using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Planora.Domain.Entities;

namespace Planora.Infrastructure.Data;

public class PlanoraDbContext : IdentityDbContext<User>
{
    public PlanoraDbContext(DbContextOptions<PlanoraDbContext> options)
        : base(options)
    {
    }

    public DbSet<Groups> Groups => Set<Groups>();
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Classrooms> Classrooms => Set<Classrooms>();
    public DbSet<Subjects> Subjects => Set<Subjects>();
    public DbSet<TeachingAssignment> TeachingAssignments => Set<TeachingAssignment>();
    public DbSet<GroupDisciplineList> GroupDisciplineLists => Set<GroupDisciplineList>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<Workload> Workloads => Set<Workload>();

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Administrator> Administrators => Set<Administrator>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(PlanoraDbContext).Assembly);
    }
}
