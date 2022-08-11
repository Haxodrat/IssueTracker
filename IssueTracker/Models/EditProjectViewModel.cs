using System;
using IssueTracker.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace IssueTracker.Models
{
    public class EditProjectViewModel
    {
        public EditProjectViewModel()
        {
            this.Users = new List<ApplicationUser>();
            this.OtherUsers = new List<ApplicationUser>();
        }

        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Status { get; set; }

        public string? ClientCompany { get; set; }

        public string? ProjectLeader { get; set; }

        public virtual List<ApplicationUser> Users { get; set; }

        public virtual List<ApplicationUser> OtherUsers { get; set; }

    }
}

