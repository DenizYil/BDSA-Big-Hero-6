using CoProject.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CoProject.Infrastructure;

public class CoProjectContext : DbContext, ICoProjectContext
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTag> ProjectTags { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<UserJoined> UserJoined { get; set; }
    public DbSet<User> Users { get; set; }

    public CoProjectContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Project>()
            .Property(project => project.State)
            .HasConversion(new EnumToStringConverter<State>());
    }
}