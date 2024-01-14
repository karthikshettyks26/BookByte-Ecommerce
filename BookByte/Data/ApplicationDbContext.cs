using BookByte.Models;
using Microsoft.EntityFrameworkCore;

namespace BookByte.Data
{
    public class ApplicationDbContext : DbContext
    {
        //Constructor
        //:base(option) is used so that whatever we configure in options will be passed to base class of 'DbContext'.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }

        //Package Manager console
        //-> cmd : update-database - it used to create/update th database.
        //whenever you want to create th table in the database-> create Dbset<classRequired>

        public DbSet<Category> Categories { get; set; }

        //In order to create table we need to do the migration.
        //package manager console -> cmd : add-migration <name of migration>
        //Once this is done Migration folder will be created and UP and DOWN mwthods will be generaed automatically in the folder.
        //use smd : update-database to reflect the table in the database.


        //used to seed the data // Initial dummy data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 }
               );
        }
    }
}
