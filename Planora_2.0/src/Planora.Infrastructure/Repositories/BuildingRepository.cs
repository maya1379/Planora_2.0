using Microsoft.EntityFrameworkCore;
using Planora.Services.Interfaces;
using Planora.Domain.Entities;
using Planora.Infrastructure.Data;

namespace Planora.Infrastructure.Repositories;

public class BuildingRepository : IBuildingRepository
{
    private readonly PlanoraDbContext _context;

    public BuildingRepository(PlanoraDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Building>> GetAllAsync()
    {
        return await _context.Buildings
            .Include(b => b.Classrooms)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Building?> GetByIdAsync(int id)
    {
        return await _context.Buildings
            .Include(b => b.Classrooms)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Building> AddAsync(Building building)
    {
        _context.Buildings.Add(building);
        await _context.SaveChangesAsync();
        return building;
    }

    public async Task UpdateAsync(Building building)
    {
        _context.Buildings.Update(building);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var building = await _context.Buildings.FindAsync(id);
        if (building != null)
        {
            _context.Buildings.Remove(building);
            await _context.SaveChangesAsync();
        }
    }
public async Task<Building?> GetWithClassroomsAndSchedulesAsync(int id)
{
    return await _context.Buildings
        .Include(b => b.Classrooms)           // Завантажуємо список аудиторій
            .ThenInclude(c => c.Schedules)    // Завантажуємо розклад для кожної з них
        .FirstOrDefaultAsync(b => b.Id == id);
}
}
