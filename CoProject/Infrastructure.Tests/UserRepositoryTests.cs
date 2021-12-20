namespace CoProject.Infrastructure.Tests;

public class UserRepositoryTests : DefaultTests
{
    private readonly UserRepository _repo;

    public UserRepositoryTests()
    {
        _repo = new(Context);
    }

    [Fact]
    public async void Create_Given_UserCreateDTO_Returns_UserDetailsDTO()
    {
        var expected = new UserDetailsDTO("2", "Wee", "wee@gmail.com", true, "/images/noimage.jpeg");

        var actual = await _repo.Create(new("2", "Wee", "wee@gmail.com", true));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void Create_User_Saves_User_To_The_DB()
    {
        var userDetails = await _repo.Create(new("2", "Wee", "wee@gmail.com", false));

        var user = await Context.Users.FirstOrDefaultAsync(u => u.Id == userDetails.Id);

        Assert.NotNull(user);
    }

    [Fact]
    public async void Read_Given_Non_Existing_User()
    {
        Assert.Null(await _repo.Read("5"));
    }

    [Fact]
    public async void Read_Given_Existing_User()
    {
        var expected = new UserDetailsDTO("1", "Myself", "me@me.dk", true, "/images/noimage.jpeg");
        Assert.Equal(expected, await _repo.Read("1"));
    }

    [Fact]
    public async void ReadAll_given_2_existing_Users_returning_List_of_2_UserDetailsDTO()
    {
        await Context.Users.AddAsync(new()
        {
            Id = "2",
            Name = "Yourself",
            Email = "you@you.dk",
            Supervisor = false,
            Image = "/images/noimage.jpeg"
        });
        await Context.SaveChangesAsync();

        var expected = new List<UserDetailsDTO>
        {
            new("1", "Myself", "me@me.dk", true, "/images/noimage.jpeg"),
            new("2", "Yourself", "you@you.dk", false, "/images/noimage.jpeg")
        };

        Assert.Equal(expected, await _repo.ReadAll());
    }

    [Fact]
    public async void ReadAllByUser_given_unexisting_id_returns_empty_list()
    {
        var expected = new List<ProjectDetailsDTO>();
        var actual = await _repo.ReadAllByUser("2");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void ReadAllByUser_given_existing_id_returns_list_of_projects()
    {
        var expected = new List<ProjectDetailsDTO>
        {
            new(
                1,
                "Default Project",
                "Default project description for tests",
                new("1", "Myself", "me@me.dk", true, "/images/noimage.jpeg"),
                State.Open,
                Now,
                new List<string>(),
                new List<UserDetailsDTO>
                {
                    new("1", "Myself", "me@me.dk", true, "/images/noimage.jpeg")
                }
            )
        };

        Project.Users = new List<User> {User};
        await Context.SaveChangesAsync();

        var actual = await _repo.ReadAllByUser("1");

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async void ReadAllByUser_given_not_supervisor_returns_users_projects()
    {
        var user = new User
        {
            Id = "2",
            Name = "Yourself",
            Email = "you@you.dk",
            Supervisor = false,
            Image = "/images/noimage.jpg"
        };

        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        var tag = new Tag
        {
            Id = 5,
            Name = "C#"
        };

        await Context.Tags.AddAsync(tag);
        await Context.SaveChangesAsync();

        Project.Tags.Add(tag);
        Project.Users = new List<User> {user};
        await Context.SaveChangesAsync();

        var expected = new List<ProjectDetailsDTO>
        {
            new(
                1,
                "Default Project",
                "Default project description for tests",
                new("1", "Myself", "me@me.dk", true, "/images/noimage.jpeg"),
                State.Open,
                Now,
                new List<string> {"C#"},
                new List<UserDetailsDTO>
                {
                    new("2", "Yourself", "you@you.dk", false, "/images/noimage.jpg")
                }
            )
        };

        var actual = await _repo.ReadAllByUser("2");

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async void Update_Given_Non_Existing_UserUpdateDTO_Returning_Status_NotFound()
    {
        var expected = Status.NotFound;
        var actual = await _repo.Update("2", new("Yourself", "you@you.dk")
        {
            Image = new ("newimage.jpg", new byte[200])
        });

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void Update_Given_Existing_UserUpdateDTO_Returning_Status_Updated()
    {
        var expected = Status.Updated;
        var actual = await _repo.Update("1", new("Yourself", "you@you.dk")
        {
            Image = new ("newimage.jpg", new byte[200])
        });

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void Update_actually_updates_User_with_specified_changes()
    {
        await _repo.Update("1", new("Yourself", "you@you.dk")
        {
            Supervisor = false,
            Image = new ("newimage.jpg", new byte[200])
            {
                Path = "images/newimage.jpg"
            }
        });

        var expected = new User
        {
            Id = "1",
            Name = "Yourself",
            Email = "you@you.dk",
            Image = "images/newimage.jpg",
            Supervisor = false
        };

        var actual = await Context.Users.FirstOrDefaultAsync(u => u.Id == "1");

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async void Delete_returns_NotFound_for_non_existing_id()
    {
        var expected = Status.NotFound;
        var actual = await _repo.Delete("5");

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void Delete_returns_Deleted_for_existing_id()
    {
        var expected = Status.Deleted;
        var actual = await _repo.Delete("1");

        Assert.Equal(expected, actual);
    }
}