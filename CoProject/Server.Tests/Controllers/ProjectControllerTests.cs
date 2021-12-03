using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace CoProject.Server.Tests.Controllers;

public class ProjectControllerTests
{
    private readonly Mock<IProjectRepository> _repository;
    private readonly ProjectController _controller;
    private readonly ProjectDetailsDTO _project;

    public ProjectControllerTests()
    {
        _repository = new();
        _controller = new(_repository.Object);
        _project = new (1, "Project Name", "Project Description", 1, State.Open, DateTime.Now, new List<string>(), new List<UserDetailsDTO>());
    }

    [Fact]
    public async void GetProjects_returns_all_projects()
    {
        // Arrange
        _repository.Setup(m => m.ReadAll()).ReturnsAsync(new List<ProjectDetailsDTO>());

        // Act
        var actual = await _controller.GetProjects();
        
        // Assert
        Assert.Equal(new List<ProjectDetailsDTO>(), actual);
    }

    [Fact]
    public async Task GetProject_returns_project_given_id()
    {
        // Arrange
        _repository.Setup(m => m.Read(1)).ReturnsAsync(_project);

        // Act
        var actual = await _controller.GetProject(1);
        
        // Assert
        Assert.Equal(_project, actual.Value);
    }
    
    [Fact]
    public async Task GetProject_returns_NotFound_given_nonexistent_id()
    {
        // Arrange
        _repository.Setup(m => m.Read(100)).ReturnsAsync(default(ProjectDetailsDTO));
        
        // Act
        var response = await _controller.GetProject(100);
        
        // Assert
        Assert.IsType<NotFoundResult>(response.Result);
    }
    
    [Fact]
    public async Task UpdateProject_given_existing_id_updates_project_and_returns_NoContent()
    {
        // Arrange
        _repository.Setup(m => m.Update(1, new ProjectUpdateDTO())).ReturnsAsync(Status.Updated);
        
        // Act
        var response = await _controller.UpdateProject(1, new ProjectUpdateDTO());

        // Assert
        Assert.IsType<NoContentResult>(response);
    }
    
    [Fact]
    public async Task UpdateProject_given_nonexistent_id_returns_NotFound()
    {
        // Arrange
        _repository.Setup(m => m.Update(1, new ProjectUpdateDTO())).ReturnsAsync(Status.NotFound);
        
        // Act
        var response = await _controller.UpdateProject(1, new ProjectUpdateDTO());

        // Assert
        Assert.IsType<NotFoundResult>(response);
    }

    [Fact]
    public async void CreateProject_creates_a_new_project()
    {
        // Arrange
        var toCreate = new ProjectCreateDTO("Project Name", "Project Description", 1, State.Open, new List<string>());
        _repository.Setup(m => m.Create(toCreate)).ReturnsAsync(_project);
        
        // Act
        var result = await _controller.CreateProject(toCreate) as CreatedAtRouteResult;
        
        // Assert
        Assert.Equal(_project, result?.Value);
        Assert.Equal("GetProject", result?.RouteName);
        Assert.Equal(KeyValuePair.Create("Id", (object?)1), result?.RouteValues?.Single());
    }

    [Fact]
    public async void DeleteProject_deletes_a_projects_given_id_and_returns_NoContent()
    {
        // Arrange
        _repository.Setup(m => m.Delete(1)).ReturnsAsync(Status.Deleted);
        
        // Act
        var response = await _controller.DeleteProject(1);

        // Assert
        Assert.IsType<NoContentResult>(response);
    }
    
    [Fact]
    public async void DeleteProject_returns_NotFound_given_nonexistent_id()
    {
        // Arrange
        _repository.Setup(m => m.Delete(10)).ReturnsAsync(Status.NotFound);
        
        // Act
        var response = await _controller.DeleteProject(10);

        // Assert
        Assert.IsType<NotFoundResult>(response);
    }

    [Fact]
    public async void AddUserToProject_given_existing_id_adds_user_to_project_and_returns_NoContent()
    {
        // Arrange
        _repository.Setup(m => m.Read(1)).ReturnsAsync(_project);
        _repository.Setup(m => m.Update(1, new ProjectUpdateDTO())).ReturnsAsync(Status.Updated);
        
        // Act
        var response = await _controller.AddUserToProject(1,1);

        // Assert
        Assert.IsType<NoContentResult>(response);
    }
    
    [Fact]
    public async void AddUserToProject_given_non_existing_id_returns_not_found()
    {
        // Arrange
        _repository.Setup(m => m.Read(1)).Returns(Task.FromResult<ProjectDetailsDTO>(null));
        _repository.Setup(m => m.Update(1, new ProjectUpdateDTO())).ReturnsAsync(Status.NotFound);
        
        // Act
        var response = await _controller.AddUserToProject(1, 1);

        // Assert
        Assert.IsType<NotFoundResult>(response);
    }

    [Fact]
    public async void RemoveUserFromProject_given_existing_id_removes_user_from_project_and_returns_NoContent()
    {
        // Arrange
        _repository.Setup(m => m.Read(1)).ReturnsAsync(_project);
        _repository.Setup(m => m.Update(1, new ProjectUpdateDTO())).ReturnsAsync(Status.Updated);
        
        // Act
        var response = await _controller.RemoveUserFromProject(1, 1);
        
        // Assert
        Assert.IsType<NoContentResult>(response);
    }
    
    [Fact]
    public async void RemoveUserFromProject_given_non_existing_id_returns_not_found()
    {
        // Arrange
        _repository.Setup(m => m.Read(1)).Returns(Task.FromResult<ProjectDetailsDTO>(null));
        _repository.Setup(m => m.Update(1, new ProjectUpdateDTO())).ReturnsAsync(Status.NotFound);
        
        // Act
        var response = await _controller.RemoveUserFromProject(1, 1);

        // Assert
        Assert.IsType<NotFoundResult>(response);
    }
}