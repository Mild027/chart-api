using Microsoft.EntityFrameworkCore;

namespace JwtWabApiTutorial.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<GraphValue> graphValues { get; set; }
    }
  
}
