using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Planora.Domain.Constants;
using Planora.Domain.Entities;
using Planora.Infrastructure.Data;
using Planora.Web.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Planora.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class AuditLogsController : Controller
{
    private readonly PlanoraDbContext _context;
    private readonly UserManager<User> _userManager;

    public AuditLogsController(PlanoraDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var logs = await _context.AuditLogs
            .OrderByDescending(a => a.DateTime)
            .ToListAsync();

        var users = await _userManager.Users.ToDictionaryAsync(u => u.Id, u => u.FullName);

        var viewModels = logs.Select(log => new AuditLogViewModel
        {
            Id = log.Id,
            UserName = log.UserId != null && users.TryGetValue(log.UserId, out var fullName) ? fullName : log.UserId ?? "System",
            Type = log.Type,
            TableName = log.TableName,
            DateTime = log.DateTime,
            OldValues = log.OldValues,
            NewValues = log.NewValues,
            AffectedColumns = log.AffectedColumns,
            PrimaryKey = log.PrimaryKey
        }).ToList();

        return View(viewModels);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Undo(int id)
    {
        var log = await _context.AuditLogs.FindAsync(id);
        if (log == null) return NotFound();

        var entityType = typeof(Planora.Domain.Entities.User).Assembly.GetTypes().FirstOrDefault(t => t.Name == log.TableName);
        if (entityType == null) return BadRequest("Невідомий тип таблиці.");

        var pkDict = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, System.Text.Json.JsonElement>>(log.PrimaryKey);
        if (pkDict == null || !pkDict.ContainsKey("Id")) return BadRequest("Не знайдено первинний ключ.");
        
        var pkProp = entityType.GetProperty("Id");
        if (pkProp == null) return BadRequest("Таблиця не має поля Id.");

        object? entityIdObj = System.Text.Json.JsonSerializer.Deserialize(pkDict["Id"].GetRawText(), pkProp.PropertyType);
        if (entityIdObj == null) return BadRequest("Недійсний первинний ключ.");

        var setMethod = _context.GetType().GetMethod("Set", Type.EmptyTypes)?.MakeGenericMethod(entityType);
        if (setMethod == null) return BadRequest("Помилка доступу до таблиці.");
        dynamic dbSet = setMethod.Invoke(_context, null)!;

        var entity = await _context.FindAsync(entityType, entityIdObj);

        try
        {
            if (log.Type == "Update")
            {
                if (entity == null) return BadRequest("Запис не знайдено, можливо він був видалений.");
                
                var oldValues = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, System.Text.Json.JsonElement>>(log.OldValues ?? "{}");
                if (oldValues != null)
                {
                    foreach (var kv in oldValues)
                    {
                        var prop = entityType.GetProperty(kv.Key);
                        if (prop != null && prop.CanWrite)
                        {
                            object? val = kv.Value.ValueKind != System.Text.Json.JsonValueKind.Null 
                                ? System.Text.Json.JsonSerializer.Deserialize(kv.Value.GetRawText(), prop.PropertyType) 
                                : null;
                            prop.SetValue(entity, val);
                        }
                    }
                }
            }
            else if (log.Type == "Delete")
            {
                if (entity != null) return BadRequest("Запис вже існує.");
                
                entity = Activator.CreateInstance(entityType);
                var oldValues = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, System.Text.Json.JsonElement>>(log.OldValues ?? "{}");
                if (oldValues != null && entity != null)
                {
                    foreach (var kv in oldValues)
                    {
                        var prop = entityType.GetProperty(kv.Key);
                        if (prop != null && prop.CanWrite)
                        {
                            object? val = kv.Value.ValueKind != System.Text.Json.JsonValueKind.Null 
                                ? System.Text.Json.JsonSerializer.Deserialize(kv.Value.GetRawText(), prop.PropertyType) 
                                : null;
                            prop.SetValue(entity, val);
                        }
                    }
                }
                dbSet.Add((dynamic)entity);
            }
            else if (log.Type == "Create")
            {
                if (entity == null) return BadRequest("Запис вже видалено.");
                dbSet.Remove((dynamic)entity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            return BadRequest($"Помилка відміни: {ex.Message}");
        }
    }
}
