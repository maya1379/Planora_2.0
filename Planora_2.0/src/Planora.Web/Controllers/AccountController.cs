using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Planora.Domain.Entities;
using Planora.Infrastructure.Data;
using Planora.Domain.Constants;
using Planora.Services.Services.Interfaces;
using Planora.Web.ViewModels;

namespace Planora.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly PlanoraDbContext _context;
    private readonly IEmailService _emailService;

    public AccountController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        PlanoraDbContext context,
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _emailService = emailService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Неправильний логін або пароль.");
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Register()
    {
        await PrepareRegisterViewData();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new User 
            { 
                UserName = model.Email, 
                Email = model.Email,
                FullName = model.FullName,
                Faculty = model.Faculty,
                Position = model.Position,
                GroupId = model.Role == AppRoles.Student ? model.GroupId : null
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        await PrepareRegisterViewData();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var callbackUrl = Url.Action(
            "ResetPassword",
            "Account",
            new { token, email = user.Email },
            protocol: Request.Scheme);

        await _emailService.SendEmailAsync(
            user.Email!,
            "Planora — Відновлення паролю",
            $@"<div style='font-family: Arial, sans-serif; max-width: 500px; margin: 0 auto; padding: 2rem; background: #1e293b; border-radius: 16px; color: #fff;'>
                <h2 style='color: #a78bfa; margin-bottom: 1rem;'>Planora</h2>
                <p>Ви отримали цей лист, бо запросили відновлення паролю.</p>
                <p>Натисніть кнопку нижче для встановлення нового паролю:</p>
                <a href='{callbackUrl}' style='display: inline-block; margin: 1.5rem 0; padding: 0.8rem 2rem; background: linear-gradient(135deg, #6366f1, #a855f7); color: #fff; text-decoration: none; border-radius: 12px; font-weight: 600;'>Змінити пароль</a>
                <p style='color: #94a3b8; font-size: 0.85rem;'>Якщо ви не запитували відновлення — просто проігноруйте цей лист.</p>
            </div>");

        return RedirectToAction(nameof(ForgotPasswordConfirmation));
    }

    [HttpGet]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string? token, string? email)
    {
        if (token == null || email == null)
            return RedirectToAction("Index", "Home");

        var model = new ResetPasswordViewModel { Token = token, Email = email };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private async Task PrepareRegisterViewData()
    {
        var groups = await _context.Groups
            .OrderBy(g => g.Name)
            .Select(g => new { g.Id, g.Name })
            .ToListAsync();
            
        ViewBag.Groups = new SelectList(groups, "Id", "Name");
        ViewBag.Roles = new SelectList(new[] { AppRoles.Admin, AppRoles.Teacher, AppRoles.Student });
    }
}
