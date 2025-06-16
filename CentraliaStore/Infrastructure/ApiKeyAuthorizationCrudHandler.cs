using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using CentraliaStore.Models;

namespace CentraliaStore.Infrastructure
{
    public class ApiKeyAuthorizationCrudHandler :
    AuthorizationHandler<OperationAuthorizationRequirement, ApiKey>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       OperationAuthorizationRequirement requirement,
                                                       ApiKey resource)
        {
            // admins can do whatever
            if (context.User.IsInRole("Administrator"))
            {
                context.Succeed(requirement);
            }

            // check can read
            if (context.User.Identity?.Name == resource.AppUser.UserName &&
                requirement.Name == Operations.Read.Name)
            {
                context.Succeed(requirement);
            }

            // check can update
            if (context.User.Identity?.Name == resource.AppUser.UserName &&
                requirement.Name == Operations.Update.Name)
            {
                context.Succeed(requirement);
            }

            // can create
            if (context.User.Identity?.Name == resource.AppUser.UserName &&
                requirement.Name == Operations.Create.Name)
            {
                context.Succeed(requirement);
            }

            // delete is left missing, only admins can delete

            return Task.CompletedTask;
        }

        public static class Operations
        {
            public static OperationAuthorizationRequirement Create =
                new OperationAuthorizationRequirement { Name = nameof(Create) };
            public static OperationAuthorizationRequirement Read =
                new OperationAuthorizationRequirement { Name = nameof(Read) };
            public static OperationAuthorizationRequirement Update =
                new OperationAuthorizationRequirement { Name = nameof(Update) };
            public static OperationAuthorizationRequirement Delete =
                new OperationAuthorizationRequirement { Name = nameof(Delete) };
        }
    }
}
