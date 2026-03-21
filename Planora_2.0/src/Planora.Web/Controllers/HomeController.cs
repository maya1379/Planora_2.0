using Microsoft.AspNetCore.Mvc;

namespace Planora.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Schedule()
    {
        return View();
    }

    public IActionResult RoomSchedule()
    {
        return View();
    }

    public IActionResult FindRoom()
    {
        return View();
    }

    public IActionResult FindPerson()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
