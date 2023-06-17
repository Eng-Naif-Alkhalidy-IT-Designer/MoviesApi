using Microsoft.EntityFrameworkCore;

namespace MoviesApi.Model
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
           
        }

        public DbSet<Genre> genres { get; set; }
        public DbSet<Movie> Movies { get; set; }
                
    }
}
