using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace CoProject.Server.Tests.Controllers;

public class ProjectControllerTests
{
    [Fact]
    public async void GetProjects_returns_all_projects()
    {
        //Arrange
        var expected = Array.Empty<ProjectDetailsDTO>();
        var repository = new Mock<IProjectRepository>();
        repository.Setup(m => m.ReadAll()).ReturnsAsync(expected);
        var controller = new ProjectController(repository.Object);
        
        
        //Act
        var actual = await controller.GetProjects();
        
        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetProject_returns_project_given_id()
    {
        //Arrange
        var repository = new Mock<IProjectRepository>();
        var project = new ProjectDetailsDTO(){Id = 1, Description = "this is a test project"};
        repository.Setup(m => m.Read(1)).ReturnsAsync(project);
        var controller = new ProjectController(repository.Object);

        //Act
        var actual = await controller.GetProject(1);
        
        //Assert
        Assert.Equal(project, actual.Value);
    }
    
    [Fact]
    public async Task GetProject_returns_notfound_given_nonexistent_id()
    {
        //Arrange
        var expected = Array.Empty<ProjectDetailsDTO>();
        var repository = new Mock<IProjectRepository>();
        repository.Setup(m => m.Read(100)).ReturnsAsync(default(ProjectDetailsDTO));
        var controller = new ProjectController(repository.Object);
        
        
        //Act
        var actual = await controller.GetProjects();
        
        //Assert
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public async Task UpdateProject_given_existing_id_updates_project()
    {
        //Arrange
        var repository = new Mock<IProjectRepository>();
        var project = new ProjectUpdateDTO();
        repository.Setup(m => m.Update(project)).ReturnsAsync(Status.Updated);
        var controller = new ProjectController(repository.Object);
        
        //Act
        var response = await controller.UpdateProject(1, project);

        //Assert
        Assert.IsType<NoContentResult>(response);
    }
    
    [Fact]
    public async Task UpdateProject_given_nonexistent_id_returns_NotFound()
    {
        //Arrange
        var repository = new Mock<IProjectRepository>();
        var project = new ProjectUpdateDTO();
        repository.Setup(m => m.Update(project)).ReturnsAsync(Status.NotFound);
        var controller = new ProjectController(repository.Object);
        
        //Act
        var response = await controller.UpdateProject(1, project);

        //Assert
        Assert.IsType<NotFoundResult>(response);
    }
    
    [Fact]
    public void CreateProject_creates_a_new_project_and_returns_status_code_201()
    {
        throw new NotImplementedException();
    }
    
    [Fact]
    public async void DeleteProject_deletes_a_projects_given_id_and_returns_status_code_204()
    {
        // Arrange
        var repository = new Mock<IProjectRepository>();
        repository.Setup(m => m.Delete(1)).ReturnsAsync(Status.Deleted);
        var controller = new ProjectController(repository.Object);

        // Act
        var response = await controller.DeleteProject(1);

        // Assert
        Assert.IsType<NoContentResult>(response);
    }
}