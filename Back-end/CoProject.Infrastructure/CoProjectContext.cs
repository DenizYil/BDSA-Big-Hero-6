using Microsoft.EntityFrameworkCore;

namespace CoProject.Infrastructure
{
    public class CoProjectContext : DbContext, ICoProjectContext
    {
        public CoProjectContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}