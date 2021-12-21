namespace Server.Integration.Tests.Tests;

public class UserTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UserTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new()
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Get_User_returns_the_logged_in_user()
    {
        var response = (await _client.GetFromJsonAsync<UserDetailsDTO>("/api/user"))!;

        Assert.NotNull(response);
        Assert.Equal("test@gmail.com", response.Email);
        Assert.Equal("1", response.Id);
        Assert.Equal("/images/noimage.jpeg", response.Image);
        Assert.Equal("Supervisor One", response.Name);
    }

    // We want this to run last
    [Fact]
    public async Task Get_projects_by_user_returns_joined_projects()
    {
        var projects = (await _client.GetFromJsonAsync<ICollection<ProjectDetailsDTO>>("api/user/projects"))!;

        Assert.NotEmpty(projects);
        Assert.Equal(3, projects.Count);
        Assert.Contains(projects, p => p.Name == "Test Project One");
    }

    [Fact]
    public async Task SignUp_returns_Success_Status_Code()
    {
        var response = await _client.PostAsJsonAsync("/api/user", "");

        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Update_user_information_returns_success_status_code()
    {
        var update = new UserUpdateDTO("Supervisor One", "test@gmail.com")
        {
            Supervisor = false
        };

        var response = await _client.PutAsJsonAsync("api/user", update);

        Assert.True(response.IsSuccessStatusCode);
    }
}