using System;
using Microsoft.AspNetCore.Identity;
using IssueTracker.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Models
{
    public class ProjectModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? User { get; set; }

    }
}

