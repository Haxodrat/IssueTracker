using System;
using System.ComponentModel.DataAnnotations;
using IssueTracker.Areas.Identity.Data;

namespace IssueTracker.Models
{
    public class TicketModel
    {

        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string? Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Priority")]
        public string? Priority { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string? Status { get; set; }

        [Required]
        [Display(Name = "Type")]
        public string? Type { get; set; }

        [Required]
        [Display(Name = "Assigned Developer")]
        public string? AssignedDeveloper { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public virtual ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "Project")]
        public virtual ProjectModel Project { get; set; }

    }
}

