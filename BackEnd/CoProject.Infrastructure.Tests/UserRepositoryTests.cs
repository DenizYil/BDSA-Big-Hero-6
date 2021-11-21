using System.Linq;
using System.Runtime.Serialization;
using FluentAssertions;

namespace CoProject.Infrastructure.Tests;

public class UserRepositoryTests
{
    private readonly ICoProjectContext _context;
    private readonly UserRepository _repo;

    public UserRepositoryTests()
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
    public async void Read_Given_Non_Existing_User()
    {
        Assert.Null(await _repo.Read(1));
    }

    [Fact]
    public async void Read_Given_Existing_User()
    {
        var user = new User
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
            NormalizedUserName = "MyselfButNormalized",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false
        };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var expected = new UserDetailsDTO()
        {
            Email = "me@me.dk",
            Name = "Myself",
            UserName = "MyselfButNormalized"
        };
        
        Assert.Equal(expected, await _repo.Read(1));

    }

    [Fact]
    public async void ReadAll_given_2_existing_Users_returning_List_of_2_UserDetailsDTO()
    {

        await _context.Users.AddRangeAsync(new User {
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
            NormalizedUserName = "MyselfButNormalized",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false
        }, 
            new User {
            Id = 2,
            Email = "you@you.dk",
            NormalizedEmail = "you@you.dk",
            Projects = new List<Project>(),
            Supervisor = false,
            EmailConfirmed = true,
            PhoneNumber = "87654321",
            LockoutEnabled = false,
            LockoutEnd = null,
            UserName = "Yourself",
            ConcurrencyStamp = "N/A",
            PasswordHash = "N/A",
            SecurityStamp = "N/A",
            AccessFailedCount = 0,
            NormalizedUserName = "YourselfButNormalized",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false
        });

        await _context.SaveChangesAsync();
        
        var expected = new List<UserDetailsDTO>
        {
            new UserDetailsDTO
            {
                Email = "me@me.dk",
                Name = "Myself",
                UserName = "MyselfButNormalized"
            },
            new UserDetailsDTO
            {
                Email = "you@you.dk",
                Name = "Yourself",
                UserName = "YourselfButNormalized"
            }
        };
        
        Assert.Equal(expected, await _repo.ReadAll());
        
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