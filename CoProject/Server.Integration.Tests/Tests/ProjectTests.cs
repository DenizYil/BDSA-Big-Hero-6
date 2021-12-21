namespace Server.Integration.Tests.Tests;

public class ProjectTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProjectTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new()
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async void Get_returns_a_list_of_projects()
    {
        var projects = await _client.GetFromJsonAsync<ICollection<ProjectDetailsDTO>>("api/projects");

        Assert.NotNull(projects);
        Assert.NotEmpty(projects);
        Assert.Contains(projects, p => p.Name == "Test Project One");
    }

    [Fact]
    public async void Get_project_by_id_returns_project()
    {
        var projectId = 1;
        var projectResponse = (await _client.GetFromJsonAsync<ProjectDetailsDTO>($"api/projects/{projectId}"))!;

        Assert.NotNull(projectResponse);
        Assert.Equal("Description for test project one", projectResponse.Description);
        Assert.Equal(1, projectResponse.Id);
        Assert.Equal(2, projectResponse.Max);
        Assert.Equal(0, projectResponse.Min);
        Assert.Equal(State.Open, projectResponse.State);
        Assert.Equal("1", projectResponse.Supervisor.Id);
        Assert.Contains(projectResponse.Tags, t => t == "MYSQL");
        Assert.Equal("Test Project One", projectResponse.Name);
    }

    [Fact]
    public async void Post_returns_project_created()
    {
        var addItem = new ProjectCreateDTO("Project three", "Description of project three", State.Open, new List<string>())
        {
            Min = 1,
            Max = 3
        };

        var response = await _client.PostAsJsonAsync("api/projects", addItem);
        var created = (await response.Content.ReadFromJsonAsync<ProjectDetailsDTO>())!;
        Assert.NotNull(created);
        Assert.Equal(addItem.Name, created.Name);
        Assert.Equal(addItem.Description, created.Description);
        Assert.Equal(addItem.State, created.State);
        Assert.Equal(addItem.Min, created.Min);
        Assert.Equal(addItem.Max, created.Max);
        Assert.Equal(4, created.Id);
    }

    [Fact]
    public async void Update_project_given_existing_id_returns_success_status_code()
    {
        var Id = 2;
        var updateProject = new ProjectUpdateDTO
        {
            Name = "Updated Project Two",
            Description = "The project description has been updated",
            Max = 5,
            Min = 2,
            Tags = new List<string> {"MYSQL", "Javascript", "SQL"},
            State = State.Open,
            Users = new List<string>()
        };
        var response = await _client.PutAsJsonAsync($"api/projects/{Id}", updateProject);

        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async void User_joins_project_given_existing_project_returns_success_status_code()
    {
        var projectId = 2;
        var response = await _client.PutAsJsonAsync($"api/projects/{projectId}/join", "");

        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async void User_leaves_project_given_existing_project_and_joined_project_returns_success_status_code()
    {
        var projectId = 1;
        var response = await _client.DeleteAsync($"api/projects/{projectId}/leave");

        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async void Delete_project_given_existing_project_returns_success_status_code()
    {
        var projectId = 3;
        var response = await _client.DeleteAsync($"api/projects/{projectId}");

        Assert.True(response.IsSuccessStatusCode);
    }
}