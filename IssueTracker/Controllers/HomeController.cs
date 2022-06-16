using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IssueTracker.Models;

namespace IssueTracker.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Projects()
    {
        return View();
    }

    public IActionResult Tickets()
    {
        return View();
    }

    public IActionResult Profile()
    {
        return View();
    }

    public IActionResult CreateTicket()
    {
        return View();
    }

    public IActionResult ViewTicket()
    {
        return View();
    }

    public IActionResult ManageRoles()
    {
        return View();
    }

    public IActionResult ManageUsers()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

