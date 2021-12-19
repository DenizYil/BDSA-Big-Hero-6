namespace CoProject.Infrastructure.Tests;

public class DefaultTests
{
    protected readonly ICoProjectContext Context;

    protected readonly DateTime Now;
    protected readonly Project Project;
    protected readonly User User;

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

        Now = DateTime.Now;

        Context = context;
        User = new()
        {
            Id = "1",
            Email = "me@me.dk",
            Projects = new List<Project>(),
            Supervisor = true,
            Name = "Myself"
        };
        Context.Users.Add(User);
        Context.SaveChanges();

        Project = new()
        {
            Id = 1,
            Name = "Default Project",
            Description = "Default project description for tests",
            SupervisorId = "1",
            Created = Now,
            State = State.Open,
            Tags = new List<Tag>(),
            Users = new List<User>()
        };
        Context.Projects.Add(Project);
        Context.SaveChanges();
    }
}