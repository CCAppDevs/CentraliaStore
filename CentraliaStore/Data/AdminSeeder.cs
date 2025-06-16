using Microsoft.AspNetCore.Identity;
using CentraliaStore.Areas.Identity;

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
                await roleManager.CreateAsync(new IdentityRole(adminRole));
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

                var result = await userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, adminRole);
                }
            }
            else if (!await userManager.IsInRoleAsync(adminUser, adminRole))
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }

            // Test User Account
            var testEmail = "test@centraliastore.com";
            var testPassword = "Test123!";
            var testUser = await userManager.FindByEmailAsync(testEmail);

            if (testUser == null)
            {
                var user = new AppUser
                {
                    UserName = testEmail,
                    Email = testEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, testPassword);

            }
        }
    }
}
