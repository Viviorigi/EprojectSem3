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

    // Ghi đè phương thức OnModelCreating nếu cần cấu hình chi tiết
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Cấu hình khóa ngoại, ràng buộc
        modelBuilder.Entity<User>()
            .HasOne(u => u.Subscription)
            .WithMany()
            .HasForeignKey(u => u.SubscriptionId);

        modelBuilder.Entity<Listing>()
            .HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserId);

        modelBuilder.Entity<Listing>()
            .HasOne(l => l.Category)
            .WithMany()
            .HasForeignKey(l => l.CategoryId);

        modelBuilder.Entity<City>()
            .HasOne(c => c.Region)
            .WithMany()
            .HasForeignKey(c => c.RegionId);
    }
}
}
