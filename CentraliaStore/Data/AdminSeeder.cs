using Microsoft.AspNetCore.Identity;
using CentraliaStore.Areas.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace CentraliaStore.Data
{
    public class AdminSeeder
    {
        public static async Task SeedAdminUser(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var adminEmail = "admin@centraliastore.com";
            var adminPassword = "Admin123!";
            var adminRole = "Administrator";

            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(adminRole));
                if (!roleResult.Succeeded)
                {
                    throw new Exception($"Failed to create role '{adminRole}': {string.Join(", ", roleResult.Errors)}");
                }
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createUserResult = await userManager.CreateAsync(user, adminPassword);
                if (!createUserResult.Succeeded)
                {
                    throw new Exception($"Failed to create admin user: {string.Join(", ", createUserResult.Errors)}");
                }

                var addToRoleResult = await userManager.AddToRoleAsync(user, adminRole);
                if (!addToRoleResult.Succeeded)
                {
                    throw new Exception($"Failed to add admin user to role: {string.Join(", ", addToRoleResult.Errors)}");
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(adminUser, adminRole))
                {
                    var addToRoleResult = await userManager.AddToRoleAsync(adminUser, adminRole);
                    if (!addToRoleResult.Succeeded)
                    {
                        throw new Exception($"Failed to add existing admin user to role: {string.Join(", ", addToRoleResult.Errors)}");
                    }
                }
            }
        }
    }
}
