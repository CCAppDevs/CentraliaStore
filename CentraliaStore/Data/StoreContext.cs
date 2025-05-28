using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CentraliaStore.Models;
using CentraliaStore.Areas.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CentraliaStore.Data
{
    public class StoreContext : IdentityDbContext
    {
        private readonly IConfiguration Configuration;

        public StoreContext(DbContextOptions<StoreContext> options, IConfiguration configuration)
            : base(options)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseSeeding((context, _) =>
                {
                    var hasher = new PasswordHasher<AppUser>();

                    var admin = context.Set<AppUser>().FirstOrDefault(u => u.UserName == Configuration["Accounts:AdminEmail"]);
                    if (admin == null)
                    {
                        context.Set<AppUser>().Add(new AppUser
                        {
                            UserName = Configuration["Accounts:AdminEmail"],
                            NormalizedUserName = Configuration["Accounts:AdminEmail"].ToUpper(),
                            Email = Configuration["Accounts:AdminEmail"],
                            NormalizedEmail = Configuration["Accounts:AdminEmail"].ToUpper(),
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            PasswordHash = hasher.HashPassword(null, Configuration["Accounts:AdminPassword"])
                        });
                        context.SaveChanges();
                    }

                    var user = context.Set<AppUser>().FirstOrDefault(u => u.UserName == Configuration["Accounts:TestUserEmail"]);
                    if (user == null)
                    {
                        context.Set<AppUser>().Add(new AppUser
                        {
                            UserName = Configuration["Accounts:TestUserEmail"],
                            NormalizedUserName = Configuration["Accounts:TestUserEmail"].ToUpper(),
                            Email = Configuration["Accounts:TestUserEmail"],
                            NormalizedEmail = Configuration["Accounts:TestUserEmail"].ToUpper(),
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            PasswordHash = hasher.HashPassword(null, Configuration["Accounts:TestUserPassword"])
                        });
                        context.SaveChanges();
                    }
                })
                .UseAsyncSeeding(async (context, _, cancellationToken) =>
                {
                    var hasher = new PasswordHasher<AppUser>();
                    var userManager = context.GetService<UserManager<AppUser>>();
                    var roleManager = context.GetService<RoleManager<IdentityRole>>();
                    var adminEmail = Configuration["Accounts:AdminEmail"];
                    var testEmail = Configuration["Accounts:TestUserEmail"];

                    if (!await roleManager.RoleExistsAsync("Admin"))
                    {
                        await roleManager.CreateAsync(new IdentityRole("Admin"));
                    }

                    var admin = await userManager.FindByEmailAsync(adminEmail);
                    if (admin == null)
                    {
                        admin = new AppUser
                        {
                            UserName = adminEmail,
                            NormalizedUserName = adminEmail.ToUpper(),
                            Email = adminEmail,
                            NormalizedEmail = adminEmail.ToUpper(),
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            PasswordHash = hasher.HashPassword(null, Configuration["Accounts:AdminPassword"])
                        };

                        await userManager.CreateAsync(admin);
                    }

                    if (!await userManager.IsInRoleAsync(admin, "Admin"))
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }

                    var user = await userManager.FindByEmailAsync(testEmail);
                    if (user == null)
                    {
                        var newUser = new AppUser
                        {
                            UserName = testEmail,
                            NormalizedUserName = testEmail.ToUpper(),
                            Email = testEmail,
                            NormalizedEmail = testEmail.ToUpper(),
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            PasswordHash = hasher.HashPassword(null, Configuration["Accounts:TestUserPassword"])
                        };

                        await userManager.CreateAsync(newUser);
                    }
                });

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Sweatshirts" },
                new Category { CategoryId = 2, Name = "Water Bottles" },
                new Category { CategoryId = 3, Name = "Notebooks" },
                new Category { CategoryId = 4, Name = "Textbooks" }
            );
        }

        public DbSet<Role> Role { get; set; } = default!;
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Phone> Phones { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<AppUser> Users { get; set; }
    }
}