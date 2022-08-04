using System;
using System.Security.Claims;
using IssueTracker.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace IssueTracker.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.Projects = new HashSet<ProjectModel>();
        }

        public String? FirstName { get; set; }

        public String? LastName { get; set; }

        public String? Role { get; set; }

        public virtual ICollection<ProjectModel> Projects { get; set; }
    }
}
