using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CoProject.Shared;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CoProject.Server.Integration.Tests;

public class UserTests: IClassFixture<CustomWebApplicationFactory>
{
    
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    
    public UserTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task get_user_returns_user()
    {
        var response = await _client.GetFromJsonAsync<UserDetailsDTO>("/api/user");
        
        Assert.NotNull(response);
        Assert.Equal("test@gmail.com", response.Email);
        Assert.Equal("1", response.Id);
        Assert.Equal("/images/noimage.jpeg", response.Image);
        Assert.Equal("Supervisor One", response.Name);
    }

    [Fact]
    public async Task get_projects_by_user()
    {
        var _projects = await _client.GetFromJsonAsync<IEnumerable<ProjectDetailsDTO>>("api/user/projects");
        
        Assert.NotEmpty(_projects);
        Assert.Equal(3, _projects.Count());
        Assert.Contains(_projects, p => p.Name == "Test Project One");
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