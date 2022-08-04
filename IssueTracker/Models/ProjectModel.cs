using System;
using Microsoft.AspNetCore.Identity;
using IssueTracker.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Models
{
    public class ProjectModel
    {
        public ProjectModel()
        {
            this.Users = new HashSet<ApplicationUser>();
        }

        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Status { get; set; }

        public string? ClientCompany { get; set; }

        public string? ProjectLeader { get; set; }

        public ICollection<ApplicationUser> Users { get; set; }

    }
}

