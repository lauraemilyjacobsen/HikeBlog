using System;
using System.Collections.Generic;
using System.Data.Entity;
using HikeBlog.Models;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace HikeBlog.Models
{
    public class BlogDbContext : DbContext
    {
        public DbSet<Location> Locations { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Comment> Comments { get; set; }

        //public DbSet<FlickrPhotoset> Photosets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}