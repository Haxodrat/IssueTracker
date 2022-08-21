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
            this.Tickets = new HashSet<TicketModel>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Required]
        [Display(Name = "Client Company")]
        public string ClientCompany { get; set; }

        [Required]
        [Display(Name = "Project Leader")]
        public string ProjectLeader { get; set; }

        [Display(Name = "Contributors")]
        public virtual ICollection<ApplicationUser> Users { get; set; }

        public virtual ICollection<TicketModel> Tickets { get; set; }

        public DateTime DateCreated { get; set; }

    }
}

