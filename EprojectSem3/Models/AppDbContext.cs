using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Reflection;
using System;

namespace EprojectSem3.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Định nghĩa các DbSet (tương ứng với các bảng trong cơ sở dữ liệu)
        public DbSet<User> Users { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }


    }
}
