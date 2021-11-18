namespace CoProject.Infrastructure.Tests;

public class ProjectRepositoryTests
{
    private readonly CoProjectContext _context;
    private readonly ProjectRepository _repo;

    public ProjectRepositoryTests()
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
        _repo = new ProjectRepository(_context);
    }
    
    [Fact]
    public async void Read_Given_Non_existing_id_returns_null()
    {
        //Arrange
        //Act
        //Assert
        Assert.Null(await _repo.Read(2));
    }
    
    [Fact]
    public async void Read_Given_Existing_Project_Returns_Project()
    {
        //Arrange
        var now = DateTime.Now;
        Project project = new Project{
            Id = 1, 
            Name = "Karl", 
            Description = "yep hehe smiley", 
            SupervisorId = 1,
            Created = now, 
            StateId = 1
        };

        ProjectDTO expectedDTO = new ProjectDTO(
            1,
            "Karl",
            "yep hehe smiley",
            now,
            1,
            null,
            null
        );
        
        //Act
        _context.Add(project);
        _context.SaveChanges();
        
        //Assert
        Assert.Equal(expectedDTO, await _repo.Read(1));
    }

    [Fact]
    public async void ReadAll_Given_Multiple_existing_Projects_Returning_All_Projects()
    {
        //Arrange
        var now = DateTime.Now;
        List<ProjectDTO> expected = new List<ProjectDTO>();
        Project projectOne = new Project{
            Id = 1, 
            Name = "Karl", 
            Description = "yep hehe smiley", 
            SupervisorId = 1,
            Created = now, 
            StateId = 1
        };
        
        Project projectTwo = new Project{
            Id = 2, 
            Name = "Phillip", 
            Description = "This is another cool description", 
            SupervisorId = 2,
            Created = now, 
            StateId = 2
        };
        ProjectDTO projectDTOOne = new ProjectDTO(
            1,
            "Karl",
            "yep hehe smiley",
            now,
            1,
            null,
            null
        );
        
        ProjectDTO projectDTOTwo = new ProjectDTO(
            2,
            "Phillip",
            "This is another cool description",
            now,
            2,
            null,
            null
        );
        //Act
        _context.Add(projectOne);
        _context.Add(projectTwo);
        _context.SaveChanges();
        
        expected.Add(projectDTOOne);
        expected.Add(projectDTOTwo);
        
        //Assert
        Assert.Equal(expected, await _repo.ReadAll());
    }
}