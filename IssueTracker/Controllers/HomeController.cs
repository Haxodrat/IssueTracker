using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IssueTracker.Models;
using Microsoft.AspNetCore.Authorization;
using IssueTracker.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using System.Net.Sockets;

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
        userManager.GetUserAsync(_context.HttpContext.User).Result.LastLogin = DateTime.Now;
        db.Users.Update(userManager.GetUserAsync(_context.HttpContext.User).Result);
        db.SaveChanges();

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

        foreach (ProjectModel project in db.Projects.Where(p => p.Users.Contains(userManager.GetUserAsync(_context.HttpContext.User).Result)))
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
        if (ModelState.IsValid)
        {
            var project = new ProjectModel
            {
                Name = Name,
                Description = Description,
                Status = Status,
                ClientCompany = ClientCompany,
                ProjectLeader = ProjectLeader,
                DateCreated = DateTime.Now
            };

            foreach (string userId in Contributors)
            {
                project.Users.Add(userManager.FindByIdAsync(userId).Result);
                userManager.FindByIdAsync(userId).Result.Projects.Add(project);
            }

            project.Users.Add(userManager.FindByIdAsync(ProjectLeader).Result);
            userManager.FindByIdAsync(ProjectLeader).Result.Projects.Add(project);

            if (!project.Users.Contains(userManager.GetUserAsync(_context.HttpContext.User).Result))
            {
                project.Users.Add(userManager.GetUserAsync(_context.HttpContext.User).Result);
            }

            db.Projects.Add(project);
            db.SaveChanges();

            return RedirectToAction("Projects");
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> EditProject(int id)
    {
        var project = await db.Projects.FindAsync(id);

        if (project == null)
        {
            return View("Error");
        }

        var model = new EditProjectViewModel()
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            ClientCompany = project.ClientCompany,
            ProjectLeader = project.ProjectLeader
        };

        var userList = (from p in db.Projects
                        where p.Id == project.Id
                        select p.Users).ToList();
        foreach (var collection in userList)
        {
            foreach (ApplicationUser user in collection)
            {
                model.Users.Add(user);
            }
        }

        foreach (ApplicationUser user in userManager.Users)
        {
            if (!model.Users.Contains(user))
            {
                model.OtherUsers.Add(user);
            }
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditProject(int Id, string Name, string Description, string Status, string ClientCompany,
                                                string ProjectLeader, List<string> Contributors)
    {
        if (ModelState.IsValid)
        {
            var project = await db.Projects.FindAsync(Id);

            if (project == null)
            {
                return View("Error");
            }
            else
            {
                project.Name = Name;
                project.Description = Description;
                project.Status = Status;
                project.ClientCompany = ClientCompany;
                project.ProjectLeader = ProjectLeader;
            }

            db.Entry(project).Collection("Users").Load();
            foreach (ApplicationUser user in project.Users)
            {
                db.Entry(user).Collection("Projects").Load();
                user.Projects.Remove(project);
                db.Users.Update(user);
            }
            project.Users.Clear();

            foreach (string Contributor in Contributors)
            {
                project.Users.Add(await userManager.FindByIdAsync(Contributor));
                userManager.FindByIdAsync(Contributor).Result.Projects.Add(project);
            }

            project.Users.Add(userManager.FindByIdAsync(ProjectLeader).Result);
            userManager.FindByIdAsync(ProjectLeader).Result.Projects.Add(project);

            db.Projects.Update(project);
            await db.SaveChangesAsync();

            return RedirectToAction("Projects");

        }

        return RedirectToAction("EditProject", Id);
    }

    [HttpGet]
    public async Task<IActionResult> ProjectDetails(int id)
    {
        var project = await db.Projects.FindAsync(id);

        if (project == null)
        {
            return View("Error");
        }

        db.Entry(project).Collection("Users").Load();
        db.Entry(project).Collection("Tickets").Load();

        return View(project);
    }

    public IActionResult Tickets()
    {
        var currentUser = userManager.GetUserAsync(_context.HttpContext?.User).Result;
        var model = (from m in db.Tickets
                     where m.User == currentUser || m.AssignedDeveloper == currentUser.Id
                     select new TicketModel
                     {
                         Id = m.Id,
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
        if (ModelState.IsValid)
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
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> EditTicket(int id)
    {
        var ticket = await db.Tickets.FindAsync(id);

        if (ticket == null)
        {
            return View("Error");
        }

        var model = new EditTicketViewModel()
        {
            Id = ticket.Id,
            Name = ticket.Name,
            Description = ticket.Description,
            Priority = ticket.Priority,
            Status = ticket.Status,
            Type = ticket.Type,
            AssignedDeveloper = ticket.AssignedDeveloper,
            DateModified = DateTime.Now
        };

        var projectList = (from t in db.Tickets
                           where t.Id == ticket.Id
                           select t.Project);
        foreach (ProjectModel project in projectList)
        {
            model.ProjectName = project.Name;
            model.ProjectId = project.Id;
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditTicket(int Id, string Name, string Description, string Priority, string Status,
                                                string Type, string AssignedDeveloper)
    {
        if (ModelState.IsValid)
        {

            var ticket = await db.Tickets.FindAsync(Id);

            if (ticket == null)
            {
                return View("Error");
            }
            else
            {
                ticket.Name = Name;
                ticket.Description = Description;
                ticket.Priority = Priority;
                ticket.Status = Status;
                ticket.Type = Type;
                ticket.AssignedDeveloper = AssignedDeveloper;
                ticket.DateModified = DateTime.Now;
            }

            db.Tickets.Update(ticket);
            db.SaveChangesAsync();

            return RedirectToAction("Tickets");

        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> TicketDetails(int id)
    {
        var ticket = await db.Tickets.FindAsync(id);

        if (ticket == null)
        {
            return View("Error");
        }

        db.Entry(ticket).Collection("Comments").Load();
        db.Entry(ticket).Reference("User").Load();
        db.Entry(ticket).Reference("Project").Load();

        return View(ticket);
    }

    [HttpPost]
    public async Task<IActionResult> TicketDetails(int id, String Comment)
    {
        if (ModelState.IsValid)
        {

            var comment = new CommentModel
            {
                Content = Comment,
                User = await userManager.GetUserAsync(_context.HttpContext.User),
                DateCreated = DateTime.Now,
                Ticket = await db.Tickets.FindAsync(id)
            };

            db.Tickets.FindAsync(id).Result.Comments.Add(comment);
            userManager.GetUserAsync(_context.HttpContext.User).Result.Comments.Add(comment);

            db.SaveChanges();

            return RedirectToAction("TicketDetails", id);

        }

        return RedirectToAction("TicketDetails", id);
    }

    public async Task<IActionResult> DeleteComment(int ticketId, int commentId)
    {
        var comment = await db.Comments.FindAsync(commentId);
        db.Entry(comment).Reference("User").Load();
        db.Entry(comment).Reference("Ticket").Load();

        comment.User.Comments.Remove(comment);
        comment.Ticket.Comments.Remove(comment);
        db.Comments.Remove(comment);

        db.SaveChanges();

        return RedirectToAction("TicketDetails", new { id = ticketId });
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
        if (ModelState.IsValid)
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

        return View();
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
        if (ModelState.IsValid)
        {
            foreach (string user in Users)
            {
                foreach (int project in Projects)
                {
                    ApplicationUser currentUser = userManager.FindByIdAsync(user).Result;
                    db.Entry(currentUser).Collection("Projects").Load();
                    ProjectModel p = db.Projects.FindAsync(project).Result;
                    if (!currentUser.Projects.Contains(p))
                    {
                        p.Users.Add(currentUser);
                        currentUser.Projects.Add(p);

                    }
                }
            }
            db.SaveChanges();

            return RedirectToAction("ManageUsers");

        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(List<string> Users)
    {
        if (ModelState.IsValid)
        {
            foreach (string userId in Users)
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
                }

                foreach (ProjectModel project in db.Projects.Where(p => p.ProjectLeader == user.Id).ToList())
                {
                    project.ProjectLeader = userManager.GetUserAsync(_context.HttpContext?.User).Result.Id;
                }
                foreach (TicketModel ticket in db.Tickets.Where(t => t.AssignedDeveloper == user.Id).ToList())
                {
                    ticket.AssignedDeveloper = userManager.GetUserAsync(_context.HttpContext?.User).Result.Id;
                }

                var result = await userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Unexpected error occurred deleting user.");
                }

                await db.SaveChangesAsync();
            }

            return RedirectToAction("ManageUsers");

        }

        return RedirectToAction("ManageUsers");

    }

    [HttpPost]
    public async Task<IActionResult> DeleteProject(int Id)
    {
        if (ModelState.IsValid)
        {
            ProjectModel project = await db.Projects.FindAsync(Id);
            db.Projects.Remove(project);
            await db.SaveChangesAsync();

            return RedirectToAction("Projects");
        }

        return RedirectToAction("ProjectDetails", Id);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteTicket(int Id)
    {
        if (ModelState.IsValid)
        {
            TicketModel ticket = await db.Tickets.FindAsync(Id);
            db.Tickets.Remove(ticket);
            await db.SaveChangesAsync();

            return RedirectToAction("Tickets");
        }

        return RedirectToAction("TicketDetails", Id);
    }

    [AllowAnonymous]
    public IActionResult MissingPage()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

