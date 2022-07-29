using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace IssueTracker.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        public String? FirstName { get; set; }

        public String? LastName { get; set; }

        public int? RoleNumber { get; set; }
    }
}

