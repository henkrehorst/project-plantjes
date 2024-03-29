﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using project_c.Models.Users;
using project_c.Models.Plants;
using project_c.Models.Chat;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;
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
        public DbSet<Report> Reports { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<PlantRating> Ratings { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");
            modelBuilder.Entity<IdentityRole>();
            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasKey(r => new {r.UserId, r.RoleId});

            base.OnModelCreating(modelBuilder);

            //  Voor de chat
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Message>()
                .HasOne<User>(a => a.User)
                .WithMany(d => d.Messages)
                .HasForeignKey(d => d.UserId);

            //add plant user relation
            modelBuilder.Entity<Plant>()
                .HasOne<User>(a => a.User)
                .WithMany(d => d.Plants)
                .HasForeignKey(d => d.UserId);
        }
        
        
    }
}