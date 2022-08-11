using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace IssueTracker.Models
{
    public class EditTicketViewModel
    {

        public int Id { get; set; }

        public string? Name { get; set; }

        public string? ProjectName { get; set; }

        public int? ProjectId { get; set; }

        public string? Description { get; set; }

        public string? Priority { get; set; }

        public string? Status { get; set; }

        public string? Type { get; set; }

        public string? AssignedDeveloper { get; set; }

        public DateTime DateModified { get; set; }


    }
}

