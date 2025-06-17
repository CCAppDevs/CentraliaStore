using Microsoft.AspNetCore.Authorization;
using CentraliaStore.Models;
using System.Security.Claims;
using System.Threading.Tasks;

public class SameOwnerRequirement : IAuthorizationRequirement { }

public class OrderAuthorizationHandler : AuthorizationHandler<SameOwnerRequirement, Order>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameOwnerRequirement requirement, Order resource)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == resource.UserId)
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
