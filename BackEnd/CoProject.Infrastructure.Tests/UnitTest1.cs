using Xunit;

namespace CoProject.Infrastructure.Tests;

public class UnitTest1
{
    private readonly ICoProjectContext _context;
    private readonly IUserRepository _repo;

    public UnitTest1()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var builder = new DbContextOptionsBuilder<CoProjectContext>();
        builder.UseSqlite(connection);
        builder.EnableSensitiveDataLogging();

        var context = new CoProjectContext(builder.Options);
        context.Database.EnsureCreated();
        context.SaveChanges();

        _context = context;
        _repo = new UserRepository(_context);
    }

    [Fact]
    public async void Delete_returns_NotFound_for_non_existing_id()
    {
        var expected = Status.NotFound;
        var actual = await _repo.Delete(1);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void Delete_returns_Deleted_for_existing_id()
    {
        // Arrange
        await _context.Users.AddAsync(new User
        {
            Id = 1,
            Email = "me@me.dk",
            NormalizedEmail = "me@me.dk",
            Projects = new List<Project>(),
            Supervisor = true,
            EmailConfirmed = true,
            PhoneNumber = "12345678",
            LockoutEnabled = false,
            LockoutEnd = null,
            UserName = "Myself",
            ConcurrencyStamp = "N/A",
            PasswordHash = "N/A",
            SecurityStamp = "N/A",
            AccessFailedCount = 0,
            NormalizedUserName = "Myself",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false
        });
        await _context.SaveChangesAsync();
    
        // Act
        var expected = Status.Deleted;
        var actual = await _repo.Delete(1);
        
        // Assert
        Assert.Equal(expected, actual);
    }
}