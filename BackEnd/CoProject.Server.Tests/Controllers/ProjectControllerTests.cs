﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace CoProject.Server.Tests.Controllers;

public class ProjectControllerTests
{
    private static readonly Mock<IProjectRepository> repository = new Mock<IProjectRepository>();
    private static readonly ProjectController controller = new ProjectController(repository.Object);

    [Fact]
    public async void GetProjects_returns_all_projects()
    {
        //Arrange
        var expected = Array.Empty<ProjectDetailsDTO>(); 
        repository.Setup(m => m.ReadAll()).ReturnsAsync(expected);

        //Act
        var actual = await controller.GetProjects();
        
        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetProject_returns_project_given_id()
    {
        //Arrange
        var project = new ProjectDetailsDTO(){Id = 1, Description = "this is a test project"};
        repository.Setup(m => m.Read(1)).ReturnsAsync(project);

        //Act
        var actual = await controller.GetProject(1);
        
        //Assert
        Assert.Equal(project, actual.Value);
    }
    
    [Fact]
    public async Task GetProject_returns_NotFound_given_nonexistent_id()
    {
        //Arrange
        repository.Setup(m => m.Read(100)).ReturnsAsync(default(ProjectDetailsDTO));
        
        
        //Act
        var response = await controller.GetProject(100);
        
        //Assert
        Assert.IsType<NotFoundResult>(response.Result);
    }
    
    [Fact]
    public async Task UpdateProject_given_existing_id_updates_project_and_returns_NoContent()
    {
        //Arrange
        var project = new ProjectUpdateDTO();
        repository.Setup(m => m.Update(1, project)).ReturnsAsync(Status.Updated);
        
        //Act
        var response = await controller.UpdateProject(1, project);

        //Assert
        Assert.IsType<NoContentResult>(response);
    }
    
    [Fact]
    public async Task UpdateProject_given_nonexistent_id_returns_NotFound()
    {
        //Arrange
        var project = new ProjectUpdateDTO();
        repository.Setup(m => m.Update(1, project)).ReturnsAsync(Status.NotFound);
        
        //Act
        var response = await controller.UpdateProject(1, project);

        //Assert
        Assert.IsType<NotFoundResult>(response);
    }

    [Fact]
    public async void CreateProject_creates_a_new_project()
    {
        //Arrange
        var toCreate = new ProjectCreateDTO();
        var project = new ProjectDetailsDTO(){Id = 1, Description = "this is a test project"};
        repository.Setup(m => m.Create(toCreate)).ReturnsAsync(project);
        
        //Act
        var result = await controller.CreateProject(toCreate) as CreatedAtRouteResult;
        
        //Assert
        Assert.Equal(project, result?.Value);
        Assert.Equal("GetProject", result?.RouteName);
        Assert.Equal(KeyValuePair.Create("Id", (object?)1), result?.RouteValues?.Single());
    }

    [Fact]
    public async void DeleteProject_deletes_a_projects_given_id_and_returns_NoContent()
    {
        // Arrange
        repository.Setup(m => m.Delete(1)).ReturnsAsync(Status.Deleted);
        
        // Act
        var response = await controller.DeleteProject(1);

        // Assert
        Assert.IsType<NoContentResult>(response);
    }
    
    [Fact]
    public async void DeleteProject_returns_NotFound_given_nonexistent_id()
    {
        // Arrange
        repository.Setup(m => m.Delete(10)).ReturnsAsync(Status.NotFound);
        
        // Act
        var response = await controller.DeleteProject(10);

        // Assert
        Assert.IsType<NotFoundResult>(response);
    }

    [Fact]
    public async void AddUserToProject_adds_user_to_project_and_returns_NoContent()
    {
        // Arrange
        repository.Setup(m => m.Read(1)).ReturnsAsync(new ProjectDetailsDTO
        {
            Id = 1,
            Users = new List<UserDetailsDTO>()
        });
        repository.Setup(m => m.Update(1, new ProjectUpdateDTO())).ReturnsAsync(Status.Updated);
        
        // Act
        var response = await controller.AddUserToProject(1,1);

        // Assert
        Assert.IsType<NoContentResult>(response);
    }

    [Fact]
    public async void RemoveUserFromProject_removes_user_from_project_and_returns_NoContent()
    {
        //Arrange
        var project = new ProjectUpdateDTO();
        repository.Setup(m => m.Read(1)).ReturnsAsync(new ProjectDetailsDTO
        {
            Users = new List<UserDetailsDTO>()
        });
        repository.Setup(m => m.Update(1, project)).ReturnsAsync(Status.Updated);
        
        //Act
        var response = await controller.RemoveUserFromProject(1, 1);
        
        //Assert
        Assert.IsType<NoContentResult>(response);
    }
}