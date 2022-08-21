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

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public string? Status { get; set; }

        [Required]
        public string? ClientCompany { get; set; }

        [Required]
        public string? ProjectLeader { get; set; }

        [Required]
        public virtual List<ApplicationUser> Users { get; set; }

        [Required]
        public virtual List<ApplicationUser> OtherUsers { get; set; }

    }
}

