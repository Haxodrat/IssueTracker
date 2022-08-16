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
            this.Tickets = new HashSet<TicketModel>();
            this.Comments = new HashSet<CommentModel>();
        }

        public String? FirstName { get; set; }

        public String? LastName { get; set; }

        public String? Role { get; set; }

        public string ProfilePhoto { get; set; }

        public virtual ICollection<ProjectModel> Projects { get; set; }

        public virtual ICollection<TicketModel> Tickets { get; set; }

        public virtual ICollection<CommentModel> Comments { get; set; }

        public DateTime LastLogin { get; set; }
    }
}