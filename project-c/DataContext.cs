using Microsoft.EntityFrameworkCore;
using project_c.Models.Users;

namespace project_c
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
        
        //add models to database with: public DbSet<modelName> name {get; set;}
        public DbSet<User> User { get; set; }
    }
}