using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IssueTracker.Models;
using Microsoft.AspNetCore.Authorization;
using IssueTracker.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Collections.Generic;

namespace IssueTracker.Controllers;

[Authorize]
public class HomeController : Controller
{
    private IssueTrackerIdentityDbContext db;
    private UserManager<ApplicationUser> userManager;
    private RoleManager<IdentityRole> roleManager;
    private IHttpContextAccessor _context;

    public HomeController(IssueTrackerIdentityDbContext db, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager, IHttpContextAccessor _context)
    {
        this.db = db;
        this.userManager = userManager;
        this.roleManager = roleManager;
        this._context = _context;
    }

    public IActionResult Index()
    {
        var model = new IndexViewModel
        {
            NoPriority = db.Tickets.Where(t => t.Priority == "None").Count(),
            LowPriority = db.Tickets.Where(t => t.Priority == "Low").Count(),
            MediumPriority = db.Tickets.Where(t => t.Priority == "Medium").Count(),
            HighPriority = db.Tickets.Where(t => t.Priority == "High").Count(),
            UrgentPriority = db.Tickets.Where(t => t.Priority == "Urgent").Count(),
            NoStatus = db.Tickets.Where(t => t.Status == "None").Count(),
            OpenStatus = db.Tickets.Where(t => t.Status == "Open").Count(),
            InProgressStatus = db.Tickets.Where(t => t.Status == "In Progress").Count(),
            ResolvedStatus = db.Tickets.Where(t => t.Status == "Resolved").Count(),
            InfoStatus = db.Tickets.Where(t => t.Status == "Additional Info Needed").Count(),
            Bugs = db.Tickets.Where(t => t.Type == "Bugs/Errors").Count(),
            Features = db.Tickets.Where(t => t.Type == "Feature Requests").Count(),
            Other = db.Tickets.Where(t => t.Type == "Other Comments").Count(),
            Styling = db.Tickets.Where(t => t.Type == "Styling Comments").Count()
        };

        var dict = new Dictionary<String, int>();

        foreach (ApplicationUser user in userManager.Users)
        {
            dict.Add(user.FirstName + " " + user.LastName, db.Tickets.Where(t => t.User == user || t.AssignedDeveloper == user.Id).Count());
        }

        dict = dict.OrderByDescending(i => i.Value).ToDictionary(i => i.Key, i => i.Value);
        dict = dict.Take(5).ToDictionary(x => x.Key, x => x.Value);

        foreach (var keyValue in dict)
        {
            model.UserTickets.Add(keyValue.Key, keyValue.Value);
        }

        return View(model);
    }

    public IActionResult Projects()
    {
        var model = new List<ProjectViewModel>();

        foreach (ProjectModel project in db.Projects)
        {
            ProjectViewModel p = new ProjectViewModel
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
            };
            var users = (from m in db.Projects
                         where m.Id == project.Id
                         select m.Users).ToList();
            foreach (var collection in users)
            {
                foreach (ApplicationUser user in collection)
                {
                    p.Users.Add(user.FirstName + " " + user.LastName);
                }

            }

            model.Add(p);
        }


        return View(model);
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
            userManager.FindByIdAsync(userId).Result.Projects.Add(project);
        }

        project.Users.Add(userManager.FindByIdAsync(ProjectLeader).Result);
        userManager.FindByIdAsync(ProjectLeader).Result.Projects.Add(project);

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
        var currentUser = userManager.GetUserAsync(_context.HttpContext?.User).Result;
        var model = (from m in db.Tickets
                     where m.User == currentUser || m.AssignedDeveloper == currentUser.Id
                     select new TicketModel
                     {
                         Name = m.Name,
                         Description = m.Description,
                         Priority = m.Priority,
                         Status = m.Status,
                         Type = m.Type,
                         AssignedDeveloper = m.AssignedDeveloper,
                         DateCreated = m.DateCreated,
                         DateModified = m.DateModified,
                         User = m.User,
                         Project = m.Project

                     }).ToList();

        return View(model);
    }

    public IActionResult CreateTicket()
    {
        return View();
    }

    [HttpPost]
    public IActionResult CreateTicket(string Title, int ProjectId, string Description, string Priority, string Status,
        string Type, string Developer)
    {
        var ticket = new TicketModel
        {
            Name = Title,
            Description = Description,
            Priority = Priority,
            Status = Status,
            Type = Type,
            AssignedDeveloper = Developer,
            DateCreated = DateTime.Now,
            DateModified = DateTime.Now
        };

        ticket.User = userManager.GetUserAsync(_context.HttpContext?.User).Result;
        ticket.Project = db.Projects.FindAsync(ProjectId).Result;
        db.Projects.FindAsync(ProjectId).Result?.Tickets.Add(ticket);

        db.Tickets.Add(ticket);
        db.SaveChanges();

        return RedirectToAction("Tickets");
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
                Id = user.Id
            };

            model.Add(userRoleViewModel);
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ManageRoles(List<string> userList, string roleName)
    {
        foreach (string userId in userList)
        {
            ApplicationUser user = await userManager.FindByIdAsync(userId);
            var roles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, roles.ToArray());
            await userManager.AddToRoleAsync(user, roleName);
            user.Role = roleName;
            await userManager.UpdateAsync(user);
        }

        return RedirectToAction("ManageRoles");
    }

    [Authorize(Roles = "Admin,Project Manager,Demo Admin,Demo Project Manager")]
    public IActionResult ManageUsers()
    {
        var model = new List<UserRoleProjectViewModel>();

        foreach (ApplicationUser user in userManager.Users)
        {
            var roleList = userManager.GetRolesAsync(user).Result;
            var projectList = (from m in db.Projects
                               from t in m.Users
                               where t.Id == user.Id
                               select new
                               {
                                   m.Name
                               }).ToList();

            List<string> projectNames = new List<string>();
            for (int i = 0; i < projectList.Count; i++)
            {
                projectNames.Add(projectList[i].Name);
            }

            var userRoleProjectViewModel = new UserRoleProjectViewModel
            {
                fullName = user.FirstName + " " + user.LastName,
                roleNames = string.Join(", ", roleList),
                Id = user.Id,
                Projects = string.Join(", ", projectNames)
            };

            model.Add(userRoleProjectViewModel);
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
                ApplicationUser currentUser = userManager.FindByIdAsync(user).Result;
                ProjectModel p = db.Projects.FindAsync(project).Result;
                p.Users.Add(currentUser);
                currentUser.Projects.Add(p);
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

