using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CentraliaStore.Models;
using CentraliaStore.Areas.Identity;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Identity;
using System.Configuration;
using System.Collections.Generic;

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

        // dynamic seeded data and configuration goes here
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseSeeding((context, _) =>
                {
                    // checking for a value in the database
                    var admin = context.Set<AppUser>().FirstOrDefault(u => u.UserName == Configuration["Accounts:AdminEmail"]);

                    var hasher = new PasswordHasher<AppUser>();

                    // if the value doesnt exist, go ahead and add it
                    if (admin == null)
                    {
                        // add a user
                        context.Set<AppUser>().Add(new AppUser
                        {
                            // add properties to specify the app user
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
                        // add a user
                        context.Set<AppUser>().Add(new AppUser
                        {
                            // add properties to specify the app user
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
                    // checking for a value in the database
                    var admin = await context.Set<AppUser>().FirstOrDefaultAsync(u => u.UserName == Configuration["Accounts:AdminEmail"]);
                    
                    var hasher = new PasswordHasher<AppUser>();

                    // if the value doesnt exist, go ahead and add it
                    if (admin == null)
                    {
                        context.Set<AppUser>().Add(new AppUser
                        {
                            // add properties to specify the app user
                            UserName = Configuration["Accounts:AdminEmail"],
                            NormalizedUserName = Configuration["Accounts:AdminEmail"].ToUpper(),
                            Email = Configuration["Accounts:AdminEmail"],
                            NormalizedEmail = Configuration["Accounts:AdminEmail"].ToUpper(),
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            PasswordHash = hasher.HashPassword(null, Configuration["Accounts:AdminPassword"])
                        });
                        await context.SaveChangesAsync(cancellationToken);

                    }

                    var user = await context.Set<AppUser>().FirstOrDefaultAsync(u => u.UserName == Configuration["Accounts:TestUserEmail"]);

                    if (user == null)
                    {
                        // add a user
                        context.Set<AppUser>().Add(new AppUser
                        {
                            // add properties to specify the app user
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
                });

        // static seeded data and model setup goes in this method
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // add categories
            builder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Sweatshirts" },
                new Category { CategoryId = 2, Name = "Water Bottles" },
                new Category { CategoryId = 3, Name = "Notebooks" },
                new Category { CategoryId = 4, Name = "Textbooks" }
            );

            // added seed data for products for shopping view
            builder.Entity<Product>().HasData(
  new Product { ProductId = 1, Name = "Logo Hoodie", Description = "Warm and comfortable", CategoryId = 1 },
  new Product { ProductId = 2, Name = "Steel Water Bottle", Description = "Keeps drinks cold", CategoryId = 2 },
  new Product { ProductId = 3, Name = "Color Changing Notebook", Description = "150 pages", CategoryId = 3 },
  new Product { ProductId = 4, Name = " c# Textbook", Description = "Intro to c#", CategoryId = 4 });
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
