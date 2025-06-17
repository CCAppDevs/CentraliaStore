using Microsoft.AspNetCore.Authorization;
using CentraliaStore.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CentraliaStore.Authorization
{
    public class ApiKeyOwnerHandler : AuthorizationHandler<ApiKeyOwnerRequirement, ApiKey>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ApiKeyOwnerRequirement requirement,
            ApiKey resource)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = context.User.IsInRole("Admin");

            if (isAdmin || resource.AppUserId == userId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
