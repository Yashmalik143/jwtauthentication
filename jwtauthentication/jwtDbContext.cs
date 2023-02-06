using Microsoft.EntityFrameworkCore;

namespace jwtauthentication
{
    public class jwtDbContext:DbContext
    {


       
            //public jwtDbContext()
            //{
            //}

            public jwtDbContext(DbContextOptions options) : base(options)
            {

            }
            public DbSet<User> logIn{ get; set; }
           
    }
 }

