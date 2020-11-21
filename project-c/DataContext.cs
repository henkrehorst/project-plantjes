using System;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;
using project_c.Models.Users;
using project_c.Models.Plants;
using project_c.Models.Chat;

namespace project_c
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        //add models to database with: public DbSet<modelName> name {get; set;}
        public DbSet<User> User { get; set; }

        public DbSet<UserData> UserData { get; set; }
        
        //added require tables for identity bundle
        public DbSet<IdentityUserClaim<Guid>> IdentityUserClaims { get; set; }
        public DbSet<IdentityUserClaim<string>> IdentityUserClaim { get; set; }

        //plant data tables
        public DbSet<Plant> Plants { get; set; }

        //chat + message data tables
        public DbSet<Chat> Chats { get; set; } 
        public DbSet<ChatData> ChatDatasets { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}