using Planora.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planora.Services.Services.Interfaces;

namespace Planora.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class ScheduleGenerationController : Controller
{
    private readonly IScheduleGenerationService _generationService;

    public ScheduleGenerationController(IScheduleGenerationService generationService)
    {
        _generationService = generationService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate()
    {
        var result = await _generationService.GenerateScheduleAsync();
        return View("Result", result);
    }
}
