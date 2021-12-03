namespace CoProject.Infrastructure.Tests;

public class DefaultTests
{
    protected readonly ICoProjectContext _context;

    protected readonly DateTime now;
    protected readonly User user;
    protected readonly Project project;
    
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

        now = DateTime.Now;

        _context = context;
        user = new User
        {
            Id = "1",
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
        
        project = new Project
        {
            Id = 1,
            Name = "Karl",
            Description = "yep hehe smiley",
            SupervisorId = 1,
            Created = now,
            State = State.Open,
            Tags = new List<Tag>(),
            Users = new List<User>()
        };
        _context.Projects.Add(project);
        _context.SaveChanges();
    }
    
}