using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CoProject.Infrastructure;

#pragma warning disable CS8618

public class CoProjectContext : DbContext, ICoProjectContext
{
    public CoProjectContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Project>()
            .Property(project => project.State)
            .HasConversion(new EnumToStringConverter<State>());

        builder.Entity<Project>()
            .HasMany(project => project.Tags)
            .WithMany(tag => tag.Projects);

        builder.Entity<Project>()
            .HasMany(project => project.Users)
            .WithMany(user => user.Projects);
    }
}