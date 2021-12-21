namespace CoProject.Server.Integration.Tests.Tests;

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
    public async Task get_user_returns_user()
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
    public async Task z_get_projects_by_user()
    {
        var projects = await _client.GetFromJsonAsync<IEnumerable<ProjectDetailsDTO>>("api/user/projects");

        Assert.NotEmpty(projects);
        Assert.Equal(3, projects.Count());
        Assert.Contains(projects, p => p.Name == "Test Project One");
    }

    [Fact]
    public async Task signup_user_returns_ok()
    {
        var response = await _client.PostAsJsonAsync("/api/user", "");

        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task update_user_returns_ok()
    {
        var update = new UserUpdateDTO("Supervisor One", "test@gmail.com")
        {
            Supervisor = false
        };

        var response = await _client.PutAsJsonAsync("api/user", update);

        Assert.True(response.IsSuccessStatusCode);
    }
}