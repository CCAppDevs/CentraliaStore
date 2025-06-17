using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CentraliaStore.Areas.Identity;
using CentraliaStore.Models;

namespace CentraliaStore.Data
{
    public class StoreContext : IdentityDbContext<AppUser>
    {
        public StoreContext(DbContextOptions<StoreContext> options)
            : base(options)
        {
        }

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
