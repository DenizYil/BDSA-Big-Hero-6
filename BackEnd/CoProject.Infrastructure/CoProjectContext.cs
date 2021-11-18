using CoProject.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoProject.Infrastructure;

public class CoProjectContext : DbContext, ICoProjectContext
{
    public DbSet<Project> Projects { get; set; }
    public CoProjectContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}