using System.Linq;
using NuGet.Frameworks;

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
        Assert.Null(await _repo.Read(2));
    }
    
    [Fact]
    public async void Read_Given_Existing_Project_Returns_Project()
    {
        //Arrange
        var now = DateTime.Now;
        
        var project = new Project{
            Id = 1, 
            Name = "Karl", 
            Description = "yep hehe smiley", 
            SupervisorId = 1,
            Created = now, 
            State = State.Open
        };

        var expected = new ProjectDTO
        {
            Id = 1,
            Name = "Karl",
            Description = "yep hehe smiley",
            SupervisorId = 1,
            Created = now,
            State = State.Open
        };
        
        _context.Add(project);
        _context.SaveChanges();
        
        // Act
        var actual = await _repo.Read(1);

        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void ReadAll_Given_Multiple_existing_Projects_Returning_All_Projects()
    {
        //Arrange
        var now = DateTime.Now;
        
        var expected = new List<ProjectDTO>
        {
            new()
            {
                Id = 1,
                Name = "Karl",
                Description = "yep hehe smiley",
                SupervisorId = 1,
                Created = now,
                State = State.Open
            },
            new()
            {
                Id = 2,
                Name = "Phillip",
                Description = "This is another cool description",
                SupervisorId = 2,
                Created = now,
                State = State.Open
            }
        };
        
        _context.Add(new Project{
            Id = 1, 
            Name = "Karl", 
            Description = "yep hehe smiley", 
            SupervisorId = 1,
            Created = now, 
            State = State.Open
        });
        _context.Add(new Project{
            Id = 2, 
            Name = "Phillip", 
            Description = "This is another cool description", 
            SupervisorId = 2,
            Created = now, 
            State = State.Open
        });
        _context.SaveChanges();
        
        // Act
        var actual = await _repo.ReadAll();

        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void Update_returns_StatusNotFound_for_non_existing_ID()
    {
        var expected = Status.NotFound;
        var actual = await _repo.Update(new ProjectUpdateDTO
        {
            Id = 2
        });
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void Update_returns_StatusUpdated_for_existing_id()
    {
        // Arrange
        var expected = Status.Updated;

        _context.Projects.Add(new Project
        {
            Id = 1,
            Name = "Phillip",
            Description = "This is another cool description",
            SupervisorId = 2,
            Created = DateTime.Now,
            State = State.Open
        });
        await _context.SaveChangesAsync();      
        
        var actual = await _repo.Update(new ProjectUpdateDTO
        {
            Id = 1,
        });
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void Update_actually_updates_object_with_specified_changes()
    {
        var now = DateTime.Now;

        var project = new Project
        {
            Id = 5,
            Name = "Phillip",
            Description = "This is another cool description",
            SupervisorId = 2,
            Created = now,
            State = State.Open
        };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var update = new ProjectUpdateDTO
        {
            Id = 5,
            Name = "Deniz",
            Description = "New description",
            State = State.Hidden
        };
        await _repo.Update(update);

        var actual = await _context.Projects.FirstOrDefaultAsync(p => p.Id == 5);
        

        var expected = new Project
        {
            Id = 5,
            Name = "Deniz",
            Description = "New description",
            State = State.Hidden,
            SupervisorId = 2,
            Created = now
        };
        
        Assert.NotNull(actual);
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Description, actual.Description);
        Assert.Equal(expected.Created, actual.Created);
        Assert.Equal(expected.SupervisorId, actual.SupervisorId);
        Assert.Equal(expected.Min, actual.Min);
        Assert.Equal(expected.Max, actual.Max);
        Assert.Equal(expected.State, actual.State); 
        
        // TODO: undersøg hvorfor man ikke bare kan assert objects
    }
    
    [Fact]
    public async void Create_Project_Given_ProjectCreateDTO_Returns_StatusCreated_and_Id()
    { 
        //Arrange
        var expected = (Status.Created, 1);
        var createProject = new ProjectCreateDTO
        {
            Name = "CoolProject",
            Description = "Description for the Coolest Project",
            Max = 4,
            State = State.Hidden,
            Tags = new List<string>()
        };
        
        //Act
        var actual = await _repo.Create(createProject);
        
        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void Create_Project_Adds_It_To_DB()
    {
        var createProject = new ProjectCreateDTO
        {
            Name = "CoolProject",
            Description = "Description for the Coolest Project",
            Max = 4,
            State = State.Hidden,
            Tags = new List<string>()
        };
        
        //Act
        var (status, id) = await _repo.Create(createProject);

        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
        Assert.NotNull(project);
    }
 
    [Fact]
    public async void Delete_Returns_Not_Found_When_Id_Doesnt_Exist()
    {
        //Arrange
        var expected = Status.NotFound;
        var actual = await _repo.Delete(3);
        
        Assert.Equal(expected,actual);
    }

    [Fact]
    public async void Delete_Changes_ProjectState_To_Deleted_When_Id_Is_Found()
    {
        var project = new Project()
        {
            Id = 1,
            Name = "Deniz sushi",
            Description = "Deniz elsker sushi",
            Created = DateTime.Now,
            SupervisorId = 2,
            State = State.Open
        };
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();

        await _repo.Delete(1);

        project = await _context.Projects.FirstOrDefaultAsync(id => project.Id == 1);

        var actual = project.State;
        var expected = State.Deleted;
        
        Assert.Equal(expected, actual);
    }
}