using System;
using Microsoft.AspNetCore.Identity;
using IssueTracker.Areas.Identity.Data;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Runtime.CompilerServices;

namespace IssueTracker.Models
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public ApplicationUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options) : base(userManager, roleManager, options)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);
            if (roles.Any())
            {
                var roleClaim = string.Join(",", roles);
                identity.AddClaim(new Claim("Role", roleClaim));
            }
            identity.AddClaim(new Claim("FirstName", user.FirstName ?? ""));
            identity.AddClaim(new Claim("LastName", user.LastName ?? ""));
            return identity;
        }
    }
}

