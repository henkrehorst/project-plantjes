using Microsoft.EntityFrameworkCore;

namespace project_c
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
        
        //add models to database with: public DbSet<modelName> name {get; set;}
    }
}