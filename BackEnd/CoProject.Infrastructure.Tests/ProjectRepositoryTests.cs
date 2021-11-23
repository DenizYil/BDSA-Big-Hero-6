using System.Linq;
using System.Runtime.Serialization;
using FluentAssertions;

namespace CoProject.Infrastructure.Tests;

public class ProjectRepositoryTests : DefaultTests
{
    private readonly ProjectRepository _repo;
    private readonly Project project;
    private readonly DateTime now = DateTime.Now;
    public ProjectRepositoryTests()
    {
        _repo = new ProjectRepository(_context);
        
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

    [Fact]
    public async void Create_Project_Given_ProjectCreateDTO_Returns_ProjectDetailsDTO()
    {
        //Arrange
        var createProject = new ProjectCreateDTO
        {
            Name = "CoolProject",
            Description = "Description for the Coolest Project",
            SupervisorId = 5,
            Min = 1,
            Max = 4,
            State = State.Hidden,
            Tags = new List<string>()
        };

        //Act
        var actual = await _repo.Create(createProject);

        var expected = new ProjectDetailsDTO
        {
            Id = 2,
            Created = actual.Created, 
            Users = new List<UserDetailsDTO>(),
            Name = "CoolProject",
            Description = "Description for the Coolest Project",
            SupervisorId = 5,
            Min = 1,
            Max = 4,
            State = State.Hidden,
            Tags = new List<string>()
        };
        //Assert
        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async void Create_Project_Saves_Project_To_Database()
    {
        var createProject = new ProjectCreateDTO
        {
            Name = "CoolProject",
            Description = "Description for the Coolest Project",
            Max = 4,
            SupervisorId = 5,
            State = State.Hidden,
            Tags = new List<string>()
        };

        //Act
        var heh = await _repo.Create(createProject);
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == heh.Id);
        Assert.NotNull(project);
    }

    [Fact]
    public async void Create_Project_With_Tags_Saves_Tags_To_Database()
    {
        var expected = new List<string> {"AI", "Python"};
        
        var createP = new ProjectCreateDTO
        {
            Name = "CoolProject",
            Description = "Description for the Coolest Project",
            Max = 4,
            SupervisorId = 5,
            State = State.Hidden,
            Tags = new List<string>(){"AI", "Python"}
        };
        var xd = await _repo.Create(createP);
        var actual = _context.Tags.Select(t => t.Name).ToList();
        
        Assert.Equal(expected, actual);
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

        var expected = new ProjectDetailsDTO
        {
            Id = 1,
            Name = "Karl",
            Description = "yep hehe smiley",
            SupervisorId = 1,
            Created = now,
            State = State.Open,
            Tags = new List<string>(),
            Users = new List<UserDetailsDTO>()
        };

        // Act
        var actual = await _repo.Read(1);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async void ReadAll_Given_Multiple_existing_Projects_Returning_All_Projects()
    {

        var expected = new List<ProjectDetailsDTO>
        {
            new()
            {
                Id = 1,
                Name = "Karl",
                Description = "yep hehe smiley",
                SupervisorId = 1,
                Created = now,
                State = State.Open,
                Tags = new List<string>(),
                Users = new List<UserDetailsDTO>()
            },
            new()
            {
                Id = 2,
                Name = "Phillip",
                Description = "This is another cool description",
                SupervisorId = 2,
                Created = now,
                State = State.Open,
                Tags = new List<string>(),
                Users = new List<UserDetailsDTO>()
            }
        };

        var newProject = project;
        newProject.Id = 2;
        newProject.Name = "Phillip";
        newProject.Description = "This is another cool description";
        newProject.SupervisorId = 2;
        
        await _context.Projects.AddAsync(newProject);
        await _context.SaveChangesAsync();

        // Act
        var actual = await _repo.ReadAll();

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async void Update_returns_StatusNotFound_for_non_existing_ID()
    {
        var expected = Status.NotFound;
        var actual = await _repo.Update(new ProjectUpdateDTO {Id = 5});
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void Update_returns_StatusUpdated_for_existing_id()
    {
        // Arrange
        var expected = Status.Updated;

        var actual = await _repo.Update(new ProjectUpdateDTO {Id = 1});

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void Update_actually_updates_object_with_specified_changes()
    {
        var projectWithTags = project;
        projectWithTags.Id = 2;
        projectWithTags.Tags = new List<Tag>
        {
            new() {Id = 2, Name = "C#", Projects = new List<Project>()},
            new() {Id = 3, Name = "F#", Projects = new List<Project>()}
        };
        projectWithTags.SupervisorId = 2;

        await _context.Projects.AddAsync(projectWithTags);
        await _context.SaveChangesAsync();
        
        var update = new ProjectUpdateDTO
        {
            Id = 2,
            Name = "Deniz",
            Min = 3,
            Max = 7,
            Tags = new List<string>{"C#"},
            Users = new List<int> {1},
            Description = "New description",
            State = State.Hidden
        };
        await _repo.Update(update);

        var actual = await _context.Projects.FirstAsync(p => p.Id == 2);

        var expected = new Project
        {
            Id = 2,
            Name = "Deniz",
            Description = "New description",
            Min = 3,
            Max = 7,
            Tags = new List<Tag>{project.Tags.ElementAt(0)},
            Users = new List<User> {
                _context.Users.First(user => user.Id == 1)
            },
            State = State.Hidden,
            SupervisorId = 2,
            Created = now
        };

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async void Delete_Returns_Not_Found_When_Id_Doesnt_Exist()
    {
        //Arrange
        var expected = Status.NotFound;
        var actual = await _repo.Delete(5);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void Delete_Changes_ProjectState_To_Deleted_When_Id_Is_Found()
    {
        await _repo.Delete(1);

        var actual = project.State;
        var expected = State.Deleted;

        Assert.Equal(expected, actual);
    }
}