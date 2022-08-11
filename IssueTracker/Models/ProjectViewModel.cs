using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace IssueTracker.Models
{
    public class ProjectViewModel
    {
        public ProjectViewModel()
        {
            this.Users = new List<string>();
        }

        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public List<string> Users { get; set; }
    }
}

