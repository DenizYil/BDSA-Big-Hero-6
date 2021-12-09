namespace CoProject.Infrastructure.Tests;

public class UserRepositoryTests : DefaultTests
{
    private readonly UserRepository _repo;

    public UserRepositoryTests()
    {
        _repo = new UserRepository(_context);
    }

    [Fact]
    public async void Create_Given_UserCreateDTO_Returns_UserDetailsDTO()
    {
        var expected = new UserDetailsDTO("2", "Wee", "wee@gmail.com", true, "/images/noimage.jpeg");

        var actual = await _repo.Create(new UserCreateDTO("2", "Wee", "wee@gmail.com", true));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void Create_User_Saves_User_To_The_DB()
    {
        var userDetails = await _repo.Create(new UserCreateDTO("2", "Wee", "wee@gmail.com", false));

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userDetails.Id);

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
        var newUser = user;
        newUser.Id = "2";
        newUser.Email = "you@you.dk";
        newUser.Name = "Yourself";

        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();

        var expected = new List<UserDetailsDTO>
        {
            new("1", "Myself", "me@me.dk", true, "/images/noimage.jpeg"),
            new("2", "Yourself", "you@you.dk", true, "/images/noimage.jpeg")
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
                "Karl",
                "yep hehe smiley",
                new UserDetailsDTO("1", "Myself", "me@me.dk", true, "/images/noimage.jpeg"),
                State.Open,
                now,
                new List<string>(),
                new List<UserDetailsDTO> {new("1", "Myself", "me@me.dk", true, "/images/noimage.jpeg") }
            )
        };

        project.Users = new List<User> {user};
        await _context.SaveChangesAsync();
        var actual = await _repo.ReadAllByUser("1");

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async void Update_Given_Non_Existing_UserUpdateDTO_Returning_Status_NotFound()
    {
        var expected = Status.NotFound;
        var actual = await _repo.Update("2", new UserUpdateDTO("YeehaaSelf", "you@you.dk"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void Update_Given_Existing_UserUpdateDTO_Returning_Status_Updated()
    {
        var expected = Status.Updated;
        var actual = await _repo.Update("1", new UserUpdateDTO("YeehaaSelf", "you@you.dk"));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void Update_actually_updates_User_with_specified_changes()
    {
        await _repo.Update("2", new UserUpdateDTO("YeehaaSelf", "you@you.dk"));

        var expected = user;
        expected.Name = "YeehaaSelf";
        expected.Email = "you@you.dk";

        var actual = await _context.Users.FirstOrDefaultAsync(u => u.Id =="1");

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