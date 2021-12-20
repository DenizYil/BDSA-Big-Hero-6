namespace CoProject.Infrastructure;

public interface ICoProjectContext : IDisposable
{
    DbSet<Project> Projects { get; }
    DbSet<Tag> Tags { get; }
    DbSet<User> Users { get; }

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}