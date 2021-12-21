using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CoProject.Infrastructure.Entities;
using CoProject.Shared;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CoProject.Server.Integration.Tests;

public class ProjectTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ProjectTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Get_returns_projects()
    {
        var projects = await _client.GetFromJsonAsync<IEnumerable<ProjectDetailsDTO>>("api/projects");
        
        Assert.NotNull(projects);
        Assert.NotEmpty(projects);
        Assert.Contains(projects, p => p.Name == "Test Project One");
    }

    [Fact]
    public async Task Get_project_by_id_returns_project()
    {
        var Id = 1;
        var projectResponse = await _client.GetFromJsonAsync<ProjectDetailsDTO>($"api/projects/{Id}");
        
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
    public async Task Post_returns_project_created()
    {
        var addItem = new ProjectCreateDTO("Project three", "Description of project three", State.Open, new List<string>())
        {
            Min = 1,
            Max = 3
        };

        var response = await _client.PostAsJsonAsync("api/projects", addItem);
        var created = await response.Content.ReadFromJsonAsync<ProjectDetailsDTO>();
        Assert.NotNull(created);
        Assert.Equal(addItem.Name, created.Name);
        Assert.Equal(addItem.Description, created.Description);
        Assert.Equal(addItem.State, created.State);
        Assert.Equal(addItem.Min, created.Min);
        Assert.Equal(addItem.Max, created.Max);
        Assert.Equal(4, created.Id);
    }

    [Fact]
    public async Task Update_returns_ok()
    {
        var Id = 2;
        var updateProject = new ProjectUpdateDTO
        {
            Name = "Updated Project Two",
            Description = "The project description has been updated",
            Max = 5,
            Min = 2,
            Tags = new List<string>() {"MYSQL", "Javascript", "SQL"},
            State = State.Open,
            Users = new List<string>()
        };
        var response = await _client.PutAsJsonAsync($"api/projects/{Id}", updateProject);
        
        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task user_joins_project_returns_ok()
    {
        var projectId = 2;
        var response = await _client.PutAsJsonAsync($"api/projects/{projectId}/join", "");
        
        Assert.True(response.IsSuccessStatusCode);
    }
    
    [Fact]
    public async Task user_leaves_project_returns_ok()
    {
        
        var projectId = 1;
        var responseLeave = await _client.DeleteAsync($"api/projects/{projectId}/leave");
        
        Assert.Equal(HttpStatusCode.OK, responseLeave.StatusCode);
    }

    [Fact]
    public async Task delete_project_returns_ok()
    {
        var projectId = 3;
        var response = await _client.DeleteAsync($"api/projects/{projectId}");
        
        Assert.True(response.IsSuccessStatusCode);
    }
}