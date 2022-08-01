using System;
using Microsoft.AspNetCore.Identity;
using IssueTracker.Areas.Identity.Data;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace IssueTracker.Models
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> userManager;

        public ApplicationUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager,
            IOptions<IdentityOptions> options) : base(userManager, options)
        {
            this.userManager = userManager;
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

