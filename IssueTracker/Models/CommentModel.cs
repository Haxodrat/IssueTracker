using System;
using System.ComponentModel.DataAnnotations;
using IssueTracker.Areas.Identity.Data;

namespace IssueTracker.Models
{
    public class CommentModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Content")]
        public string? Content { get; set; }

        [Required]
        [Display(Name = "User")]
        public virtual ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "Ticket")]
        public virtual TicketModel Ticket { get; set; }


    }
}

