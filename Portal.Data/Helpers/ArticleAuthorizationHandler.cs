using Microsoft.AspNetCore.Authorization;
using Portal.Data.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Data.Helpers
{
    public class ArticleAuthorizationHandler : AuthorizationHandler<SameArticleRequirement, Article>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       SameArticleRequirement requirement,
                                                       Article resource)
        {
            if (resource.User.EditorId != null)
            {
                if ((context.User.FindFirst(ClaimTypes.NameIdentifier).Value.Equals(resource.User.EditorId.ToString()) &&
                context.User.IsInRole("Editor")))
                {
                    context.Succeed(requirement);
                }
            }
            if ((context.User.FindFirst(ClaimTypes.NameIdentifier).Value.Equals(resource.UserId.ToString()) &&
                context.User.IsInRole("Journalist")) || context.User.IsInRole("Administrator"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
    public class SameArticleRequirement : IAuthorizationRequirement { }
}
