using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Planora.Domain.Entities;
using Planora.Domain.Enums;
using Planora.Services.Services.Interfaces;
using Planora.Web.ViewModels;

namespace Planora.Web.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IGroupService _groupService;

    public UsersController(UserManager<User> userManager, IGroupService groupService)
    {
        _userManager = userManager;
        _groupService = groupService;
    }

    public IActionResult Index()
    {
        var users = _userManager.Users.OrderBy(u => u.FullName).ToList();
        return View(users);
    }

    public async Task<IActionResult> Create()
    {
        var groups = await _groupService.GetAllAsync();
        ViewBag.Groups = new SelectList(groups, "Id", "Name");
        return View(new CreateUserViewModel { Role = UserRole.Student });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (!ModelState.IsValid) 
        {
            var groups = await _groupService.GetAllAsync();
            ViewBag.Groups = new SelectList(groups, "Id", "Name");
            return View(model);
        }

        var user = new User
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            Role = model.Role,
            Faculty = model.Faculty,
            Position = model.Position,
            GroupId = model.Role == UserRole.Student ? model.GroupId : null,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, model.Role.ToString());
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var groups = await _groupService.GetAllAsync();
        ViewBag.Groups = new SelectList(groups, "Id", "Name");

        var model = new EditUserViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Faculty = user.Faculty,
            Position = user.Position,
            GroupId = user.GroupId,
            Role = user.Role
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var groups = await _groupService.GetAllAsync();
            ViewBag.Groups = new SelectList(groups, "Id", "Name");
            return View(model);
        }

        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null) return NotFound();

        user.FullName = model.FullName;
        user.Faculty = model.Faculty;
        user.Position = model.Position;
        user.GroupId = user.Role == UserRole.Student ? model.GroupId : null;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }
        return RedirectToAction(nameof(Index));
    }
}
