using Microsoft.AspNetCore.Authorization;
using Portal.Data.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Data.Helpers
{
    public class UserAuthorizationHandler : AuthorizationHandler<SamePortalUserRequirement, User>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SamePortalUserRequirement requirement, User resource)
        {
            if ((context.User.FindFirst(ClaimTypes.NameIdentifier).Value.Equals(resource.Id.ToString()) &&
                context.User.Identity.IsAuthenticated || context.User.IsInRole("Administrator")))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
    public class SamePortalUserRequirement : IAuthorizationRequirement { }
}

