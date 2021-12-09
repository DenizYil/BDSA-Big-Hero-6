namespace CoProject.Infrastructure.Tests;

public class ProjectRepositoryTests : DefaultTests
{
    private readonly ProjectRepository _repo;

    public ProjectRepositoryTests()
    {
        _repo = new ProjectRepository(_context);
    }

    [Fact]
    public async void Create_Project_Given_ProjectCreateDTO_Returns_ProjectDetailsDTO()
    {
        //Arrange
        var createProject = new ProjectCreateDTO("CoolProject", "Description for the Coolest Project", State.Hidden, new List<string>())
        {
            SupervisorId = "1",
            Min = 1,
            Max = 4
        };

        //Act
        var actual = await _repo.Create(createProject);

        var expected = new ProjectDetailsDTO(2, "CoolProject", "Description for the Coolest Project", new UserDetailsDTO("1", "Myself", "me@me.dk", true, "/images/noimage.jpeg"), State.Hidden, actual.Created, new List<string>(), new List<UserDetailsDTO>())
        {
            Min = 1,
            Max = 4,
        };

        //Assert
        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async void Create_Project_Saves_Project_To_Database()
    {
        //Arrange
        var createProject = new ProjectCreateDTO("CoolProject", "Description for the Coolest Project", State.Hidden, new List<string>())
        {
            SupervisorId = "1",
            Min = 1,
            Max = 4
        };

        //Act
        var created = await _repo.Create(createProject);
        Assert.NotNull(await _context.Projects.FirstOrDefaultAsync(p => p.Id == created.Id));
    }

    [Fact]
    public async void Create_Project_With_Tags_Saves_Tags_To_Database()
    {
        var expected = new List<string> {"AI", "Python"};

        var createProject = new ProjectCreateDTO("CoolProject", "Description for the Coolest Project", State.Hidden, new List<string> {"AI", "Python"})
        {
            SupervisorId = "1",
            Min = 1,
            Max = 4
        };

        await _repo.Create(createProject);
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
        var expected = new ProjectDetailsDTO(1, "Karl", "yep hehe smiley", new UserDetailsDTO("1", "Myself", "me@me.dk", true, "/images/noimage.jpeg"), State.Open, now, new List<string>(), new List<UserDetailsDTO>());

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
            new(1, "Karl", "yep hehe smiley", new UserDetailsDTO("1", "Myself", "me@me.dk", true, "/images/noimage.jpeg"), State.Open, now, new List<string>(), new List<UserDetailsDTO>()),
            new(2, "Phillip", "This is another cool description", new UserDetailsDTO("2", "Yourself", "you@you.dk", true, "/images/noimage.jpeg"), State.Open, now, new List<string>(),
                new List<UserDetailsDTO>())
        };
        
        var newUser = user;
        newUser.Id = "2";
        newUser.Email = "you@you.dk";
        newUser.Name = "Yourself";

        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();

        var newProject = project;
        newProject.Id = 2;
        newProject.Name = "Phillip";
        newProject.Description = "This is another cool description";
        newProject.SupervisorId = "2";

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
        var actual = await _repo.Update(2, new ProjectUpdateDTO());

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void Update_returns_StatusUpdated_for_existing_id()
    {
        // Arrange
        var expected = Status.Updated;
        var actual = await _repo.Update(1, new ProjectUpdateDTO());

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void Update_actually_updates_object_with_specified_changes()
    {
        var otherUser = user;
        otherUser.Id = "3";
        
        await _context.Users.AddAsync(otherUser);
        await _context.SaveChangesAsync();
        
        var projectWithTags = project;
        projectWithTags.Id = 2;
        projectWithTags.Tags = new List<Tag>
        {
            new() {Id = 2, Name = "C#", Projects = new List<Project>()},
            new() {Id = 3, Name = "F#", Projects = new List<Project>()}
        };
        projectWithTags.SupervisorId = "2";
        projectWithTags.Users = new List<User> {user, otherUser};

        await _context.Projects.AddAsync(projectWithTags);
        await _context.SaveChangesAsync();

        var update = new ProjectUpdateDTO
        {
            Name = "Deniz",
            Min = 3,
            Max = 7,
            Tags = new List<string> {"C#"},
            Users = new List<string> {"1"},
            Description = "New description",
            State = State.Hidden
        };
        await _repo.Update(2, update);

        var actual = await _context.Projects.FirstAsync(p => p.Id == 2);

        var expected = new Project
        {
            Id = 2,
            Name = "Deniz",
            Description = "New description",
            Min = 3,
            Max = 7,
            Tags = new List<Tag> {project.Tags.ElementAt(0)},
            Users = new List<User> {_context.Users.First(user => user.Id.Equals("1"))},
            State = State.Hidden,
            SupervisorId = "2",
            Created = now
        };

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async void Update_adds_user_to_project()
    {
        var newUser = user;
        newUser.Id = "2";
        newUser.Email = "you@you.dk";
        newUser.Name = "Yourself";

        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();

        var users = await _context.Users.Select(u => u.Id).ToListAsync();
        users.Add("2");

        await _repo.Update(1, new ProjectUpdateDTO {Users = users});

        var expected = new List<string> {"1", "2"};
        var actual = (await _context.Projects.FirstAsync(p => p.Id == 1)).Users.Select(u => u.Id);
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void Update_removes_user_from_project()
    {
        await _repo.Update(1, new ProjectUpdateDTO {Users = new List<string>()});

        var expected = new List<string>();
        var actual = (await _context.Projects.FirstAsync()).Users.Select(u => u.Id);
        
        Assert.Equal(expected, actual);
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