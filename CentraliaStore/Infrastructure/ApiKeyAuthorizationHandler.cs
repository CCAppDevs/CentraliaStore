using CentraliaStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace CentraliaStore.Infrastructure
{
    public class ApiKeyAuthorizationHandler :
    AuthorizationHandler<SameAuthorRequirement, ApiKey>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       SameAuthorRequirement requirement,
                                                       ApiKey resource)
        {
            if(context.User.IsInRole("Administrator"))
            {
                context.Succeed(requirement);
            }

            if (context.User.Identity?.Name == resource.AppUser.UserName)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class SameAuthorRequirement : IAuthorizationRequirement { }
}
