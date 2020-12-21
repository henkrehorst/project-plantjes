using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using project_c.Models.Users;
using project_c.Models.Plants;
using project_c.Models.Chat;
using System.ComponentModel.DataAnnotations.Schema;

namespace project_c
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {
        }

        //add models to database with: public DbSet<modelName> name {get; set;}
        public DbSet<User> User { get; set; }

        //added require tables for identity bundle
        public DbSet<IdentityUserClaim<Guid>> IdentityUserClaims { get; set; }
        public DbSet<IdentityUserClaim<string>> IdentityUserClaim { get; set; }
        public DbSet<IdentityRole<string>> IdentityRoles { get; set; }
        public DbSet<IdentityUserRole<string>> IdentityUserRoles { get; set; }
        public DbSet<IdentityRoleClaim<string>> IdentityRoleClaim { get; set; }

        //plant data tables
        public DbSet<Plant> Plants { get; set; }
        public DbSet<Filter> Filters { get; set; }
        public DbSet<Option> Options { get; set; }

        //chat + message data tables
        public DbSet<Chat> Chats { get; set; } 
        public DbSet<ChatData> ChatDatasets { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<PlantRating> Ratings { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");
            modelBuilder.Entity<IdentityRole>();
            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasKey(r => new {r.UserId, r.RoleId});

            base.OnModelCreating(modelBuilder);
        }
        
        
    }
}