using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using CentraliaStore.Models;

namespace CentraliaStore.Authorization
{
    public class OrderAuthorizationHandler : AuthorizationHandler<OrderOwnerRequirement, Order>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OrderOwnerRequirement requirement,
            Order resource)
        {
            if (context.User.IsInRole("Administrator") ||
                context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value == resource.UserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
