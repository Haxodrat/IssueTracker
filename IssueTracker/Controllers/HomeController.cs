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
    private IssueTrackerIdentityDbContext db;
    private UserManager<ApplicationUser> userManager;
    private RoleManager<IdentityRole> roleManager;

    public HomeController(IssueTrackerIdentityDbContext db, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        this.db = db;
        this.userManager = userManager;
        this.roleManager = roleManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Projects()
    {
        return View();
    }

    public IActionResult CreateProject()
    {
        return View();
    }

    [HttpPost]
    public IActionResult CreateProject(String Name, String Description, String Status,
        String ClientCompany, String ProjectLeader, List<string> Contributors)
    {
        var project = new ProjectModel
        {
            Name = Name,
            Description = Description,
            Status = Status,
            ClientCompany = ClientCompany,
            ProjectLeader = ProjectLeader
        };

        foreach (string userId in Contributors)
        {
            project.Users.Add(userManager.FindByIdAsync(userId).Result);
        }

        db.Projects.Add(project);
        db.SaveChanges();

        return RedirectToAction("Projects");
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

    [Authorize(Roles = "Admin,Demo Admin")]
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
                roleNames = roles,
                email = user.Email
            };

            model.Add(userRoleViewModel);
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ManageRoles(List<string> userList, string roleName)
    {
        foreach (string userEmail in userList)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(userEmail);
            var roles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, roles.ToArray());
            await userManager.AddToRoleAsync(user, roleName);
        }

        return RedirectToAction("ManageRoles");
    }

    [Authorize(Roles = "Admin,Project Manager,Demo Admin,Demo Project Manager")]
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
                roleNames = roles,
                email = user.Email
            };

            model.Add(userRoleViewModel);
        }

        return View(model);
    }

    [HttpPost]
    public IActionResult ManageUsers(List<string> Users, List<int> Projects)
    {
        foreach (string user in Users)
        {
            foreach (int project in Projects)
            {
                ProjectModel p = db.Projects.FindAsync(project).Result;
                p.Users.Add(userManager.FindByEmailAsync(user).Result);
            }
        }
        db.SaveChanges();
        return RedirectToAction("ManageUsers");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

