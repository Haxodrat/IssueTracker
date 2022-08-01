using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IssueTracker.Models;
using Microsoft.AspNetCore.Authorization;
using IssueTracker.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IssueTracker.Controllers;

[Authorize]
public class HomeController : Controller
{
    private IssueTrackerIdentityDbContext application;
    private UserManager<ApplicationUser> userManager;
    private RoleManager<IdentityRole> roleManager;

    public HomeController(IssueTrackerIdentityDbContext app, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        application = app;
        this.userManager = userManager;
        this.roleManager = roleManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Roles = "Demo Submitter")]
    public IActionResult Projects()
    {
        return View();
    }

    public IActionResult CreateProject()
    {
        return View();
    }

    public IActionResult ProjectDetails()
    {
        return View();
    }

    public IActionResult EditProject()
    {
        return View();
    }

    public IActionResult Tickets()
    {
        return View();
    }

    public IActionResult CreateTicket()
    {
        return View();
    }

    public IActionResult EditTicket()
    {
        return View();
    }

    public IActionResult TicketDetails()
    {
        return View();
    }

    public IActionResult Profile()
    {
        return View();
    }


    public IActionResult ManageRoles()
    {
        var model = new List<UserRoleViewModel>();

        foreach (var user in userManager.Users)
        {
            var roleList = userManager.GetRolesAsync(user).Result;
            var roles = string.Join(", ", roleList);

            var userRoleViewModel = new UserRoleViewModel
            {
                userName = user.FirstName + " " + user.LastName,
                roleNames = roles
            };

            model.Add(userRoleViewModel);
        }

        return View(model);
    }

    public IActionResult ManageUsers()
    {
        var model = new List<UserRoleViewModel>();

        foreach (var user in userManager.Users)
        {
            var roleList = userManager.GetRolesAsync(user).Result;
            var roles = string.Join(", ", roleList);

            var userRoleViewModel = new UserRoleViewModel
            {
                userName = user.FirstName + " " + user.LastName,
                roleNames = roles
            };

            model.Add(userRoleViewModel);
        }

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

