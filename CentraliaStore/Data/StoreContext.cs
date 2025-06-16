using CentraliaStore.Areas.Identity;
using CentraliaStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CentraliaStore.Data
{
    public class StoreContext : IdentityDbContext<AppUser>
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
                    var admin = context.Set<AppUser>().FirstOrDefault(u => u.UserName == Configuration["Accounts:AdminEmail"]);
                    var hasher = new PasswordHasher<AppUser>();

                    if (admin == null)
                    {
                        admin = new AppUser
                        {
                            UserName = Configuration["Accounts:AdminEmail"],
                            NormalizedUserName = Configuration["Accounts:AdminEmail"].ToUpper(),
                            Email = Configuration["Accounts:AdminEmail"],
                            NormalizedEmail = Configuration["Accounts:AdminEmail"].ToUpper(),
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            PasswordHash = hasher.HashPassword(null, Configuration["Accounts:AdminPassword"])
                        };
                        context.Set<AppUser>().Add(admin);
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

                    var adminRole = context.Set<IdentityRole>().FirstOrDefault(r => r.Name == "Administrator");

                    if (adminRole == null)
                    {
                        string roleId = Guid.NewGuid().ToString();

                        adminRole = new IdentityRole
                        {
                            Id = roleId,
                            Name = "Administrator",
                            NormalizedName = "ADMINISTRATOR",
                            ConcurrencyStamp = roleId,
                        };

                        context.Set<IdentityRole>().Add(adminRole);
                        context.SaveChanges();
                    }

                    var roleUser = context.Set<IdentityUserRole<string>>().FirstOrDefault(a => a.RoleId == adminRole.Id);

                    if (roleUser == null)
                    {
                        roleUser = new IdentityUserRole<string>
                        {
                            RoleId = adminRole.Id,
                            UserId = admin.Id
                        };

                        context.Set<IdentityUserRole<string>>().Add(roleUser);
                        context.SaveChanges();
                    }

                    var adminApiKey = context.Set<ApiKey>().FirstOrDefault(k => k.AppUserId == admin.Id);
                    if (adminApiKey == null)
                    {
                        adminApiKey = new ApiKey
                        {
                            ApiSecret = Guid.NewGuid().ToString(),
                            AppUserId = admin.Id
                        };

                        context.Set<ApiKey>().Add(adminApiKey);
                        context.SaveChanges();
                    }

                    var userApiKey = context.Set<ApiKey>().FirstOrDefault(k => k.AppUserId == user.Id);
                    if (userApiKey == null)
                    {
                        userApiKey = new ApiKey
                        {
                            ApiSecret = Guid.NewGuid().ToString(),
                            AppUserId = user.Id
                        };

                        context.Set<ApiKey>().Add(userApiKey);
                        context.SaveChanges();
                    }
                })
                .UseAsyncSeeding(async (context, _, cancellationToken) =>
                {
                    var admin = await context.Set<AppUser>().FirstOrDefaultAsync(u => u.UserName == Configuration["Accounts:AdminEmail"]);
                    var hasher = new PasswordHasher<AppUser>();

                    if (admin == null)
                    {
                        admin = new AppUser
                        {
                            UserName = Configuration["Accounts:AdminEmail"],
                            NormalizedUserName = Configuration["Accounts:AdminEmail"].ToUpper(),
                            Email = Configuration["Accounts:AdminEmail"],
                            NormalizedEmail = Configuration["Accounts:AdminEmail"].ToUpper(),
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            PasswordHash = hasher.HashPassword(null, Configuration["Accounts:AdminPassword"])
                        };
                        context.Set<AppUser>().Add(admin);
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    var user = await context.Set<AppUser>().FirstOrDefaultAsync(u => u.UserName == Configuration["Accounts:TestUserEmail"]);

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
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    var adminRole = await context.Set<IdentityRole>().FirstOrDefaultAsync(r => r.Name == "Administrator");

                    if (adminRole == null)
                    {
                        string roleId = Guid.NewGuid().ToString();

                        adminRole = new IdentityRole
                        {
                            Id = roleId,
                            Name = "Administrator",
                            NormalizedName = "ADMINISTRATOR",
                            ConcurrencyStamp = roleId,
                        };

                        context.Set<IdentityRole>().Add(adminRole);
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    var roleUser = await context.Set<IdentityUserRole<string>>().FirstOrDefaultAsync(a => a.RoleId == adminRole.Id);

                    if (roleUser == null)
                    {
                        roleUser = new IdentityUserRole<string>
                        {
                            RoleId = adminRole.Id,
                            UserId = admin.Id
                        };

                        context.Set<IdentityUserRole<string>>().Add(roleUser);
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    var adminApiKey = await context.Set<ApiKey>().FirstOrDefaultAsync(k => k.AppUserId == admin.Id);
                    if (adminApiKey == null)
                    {
                        adminApiKey = new ApiKey
                        {
                            ApiSecret = Guid.NewGuid().ToString(),
                            AppUserId = admin.Id
                        };

                        context.Set<ApiKey>().Add(adminApiKey);
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    var userApiKey = await context.Set<ApiKey>().FirstOrDefaultAsync(k => k.AppUserId == user.Id);
                    if (userApiKey == null)
                    {
                        userApiKey = new ApiKey
                        {
                            ApiSecret = Guid.NewGuid().ToString(),
                            AppUserId = user.Id
                        };

                        context.Set<ApiKey>().Add(userApiKey);
                        await context.SaveChangesAsync(cancellationToken);
                    }
                });

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Sweatshirts" },
                new Category { CategoryId = 2, Name = "Water Bottles" },
                new Category { CategoryId = 3, Name = "Notebooks" },
                new Category { CategoryId = 4, Name = "Textbooks" },
                new Category { CategoryId = 5, Name = "Writing Utensils" }
            );

            builder.Entity<Product>().HasData(
                new Product { ProductId = 1, Name = "Logo Hoodie", Description = "Warm and comfortable", CategoryId = 1 },
                new Product { ProductId = 2, Name = "Steel Water Bottle", Description = "Keeps drinks cold", CategoryId = 2 },
                new Product { ProductId = 3, Name = "Color Changing Notebook", Description = "150 pages", CategoryId = 3 },
                new Product { ProductId = 4, Name = "C# Textbook", Description = "Intro to C#", CategoryId = 4 }
            );

            builder.Entity<ApiKey>().Navigation(k => k.AppUser).AutoInclude();
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Phone> Phones { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<IdentityRole> Roles { get; set; } = default!;
    }
}
