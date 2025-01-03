﻿using Microsoft.EntityFrameworkCore;

namespace  DataAccessLayer_DAL.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Statistical> Statisticals { get; set; }
        public DbSet<BookMark> BookMarks { get; set; }
        public DbSet<Rating> Ratings { get; set; }
    }
}
