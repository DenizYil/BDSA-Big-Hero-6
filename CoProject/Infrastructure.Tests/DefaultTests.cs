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
            Projects = new List<Project>(),
            Supervisor = true,
            Name = "Myself"
        };
        _context.Users.Add(user);
        _context.SaveChanges();

        project = new Project
        {
            Id = 1,
            Name = "Karl",
            Description = "yep hehe smiley",
            SupervisorId = "1",
            Created = now,
            State = State.Open,
            Tags = new List<Tag>(),
            Users = new List<User>()
        };
        _context.Projects.Add(project);
        _context.SaveChanges();
    }
}