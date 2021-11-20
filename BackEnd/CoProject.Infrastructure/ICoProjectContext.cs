using CoProject.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoProject.Infrastructure;

public interface ICoProjectContext : IDisposable
{
    DbSet<Project> Projects { get; set; }
    DbSet<Tag> Tags { get; set; }
    DbSet<User> Users { get; set; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}