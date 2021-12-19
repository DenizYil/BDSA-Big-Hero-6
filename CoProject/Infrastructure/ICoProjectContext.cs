using CoProject.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CoProject.Infrastructure;

public interface ICoProjectContext : IDisposable
{
    DbSet<Project> Projects { get; set; }
    DbSet<Tag> Tags { get; set; }
    DbSet<User> Users { get; set; }

    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
}