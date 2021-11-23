namespace CoProject.Infrastructure.Tests;

public class DefaultTests
{
    protected readonly ICoProjectContext _context;

    protected readonly User user;
        protected DefaultTests()
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
        user = new User
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
        _context.Users.Add(user);
        _context.SaveChanges();
    }
    
}