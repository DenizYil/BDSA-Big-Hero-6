using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using CoProject.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

namespace CoProject.Server.Tests.Controllers;

public class ProjectControllerTests
{
    private readonly Mock<IProjectRepository> _projectRepository;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly ProjectController _controller;
    private readonly ProjectDetailsDTO _project;
    private readonly UserDetailsDTO _user;
    private readonly UserDetailsDTO _supervisor;
    private readonly GenericIdentity _identity;
    

    public ProjectControllerTests()
    {
        
        _projectRepository = new();
        _userRepository = new ();
        _project = new(1, "Project Name", "Project Description", _supervisor, State.Open, DateTime.Now, new List<string>(), new List<UserDetailsDTO>());
        _user = new("123", "Test User", "user@outlook.com", false, "/images/noimage.jpeg");
        _supervisor = new UserDetailsDTO("1", "Supervisor", "supervisor@outlook.com", true, "/images/noimage.jpeg");
        
        _identity = new GenericIdentity("Identity User", "");
        _identity.AddClaim(new Claim(ClaimConstants.ObjectId, _user.Id));
        _controller = new(_projectRepository.Object, _userRepository.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new GenericPrincipal(_identity, new string[] { }))
                }
            }
        };
    }

    [Fact]
    public async void GetProjects_returns_all_projects()
    {
        // Arrange
        var expected = Array.Empty<ProjectDetailsDTO>();
        _projectRepository.Setup(m => m.ReadAll()).ReturnsAsync(expected);

        // Act
        var actual = await _controller.GetProjects();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetProject_returns_project_given_id()
    {
        // Arrange
        _projectRepository.Setup(m => m.Read(1)).ReturnsAsync(_project);

        // Act
        var actual = await _controller.GetProject(1);

        // Assert
        Assert.Equal(_project, actual.Value);
    }

    
    
    [Fact]
    public async Task GetProject_returns_NotFoundObject_given_nonexistent_id()
    {
        // Arrange
        _projectRepository.Setup(m => m.Read(100)).ReturnsAsync(default(ProjectDetailsDTO));

        // Act
        var response = await _controller.GetProject(100);

        // Assert
        Assert.IsType<NotFoundObjectResult>(response.Result);
    }
    
    
    //TODO: Make it able to "read" and pass all if statements in UpdateProject()
    [Fact]
    public async Task UpdateProject_given_existing_id_updates_project_and_returns_NoContent()
    {
        // Arrange
        _projectRepository.Setup(m => m.Update(1, new ProjectUpdateDTO())).ReturnsAsync(Status.Updated);
        _projectRepository.Setup(m => m.Read(1)).ReturnsAsync(_project);
        _userRepository.Setup(m => m.Read("1")).ReturnsAsync(_supervisor);

        // Act
        var response = await _controller.UpdateProject(1, new ProjectUpdateDTO());

        // Assert
        Assert.IsType<OkObjectResult>(response);
    }
    
    
    [Fact]
    public async Task UpdateProject_given_nonexistent_id_returns_NotFound()
    {
        // Arrange
        _projectRepository
            .Setup(m => m.Update(1, new ProjectUpdateDTO()))
            .ReturnsAsync(Status.NotFound);

        // Act
        var response = await _controller.UpdateProject(1, new ProjectUpdateDTO());

        // Assert
        Assert.IsType<NotFoundObjectResult>(response);
    }

    
    //TODO: make test pass all if statements in CreateProject()
    [Fact]
    public async void CreateProject_creates_a_new_project()
    {
        // Arrange
        var toCreate = new ProjectCreateDTO("Project Name", "Project Description", State.Open, new List<string>());
        _projectRepository.Setup(m => m.Create(toCreate)).ReturnsAsync(_project);

        // Act
        var result = await _controller.CreateProject(toCreate) as CreatedAtRouteResult;

        // Assert
        Assert.Equal(_project, result?.Value);
        Assert.Equal("GetProject", result?.RouteName);
        Assert.Equal(KeyValuePair.Create("Id", (object?) 1), result?.RouteValues?.Single());
    }

    [Fact]
    public async void DeleteProject_deletes_a_projects_given_id_and_returns_NoContent()
    {
        // Arrange
        _projectRepository.Setup(m => m.Delete(1)).ReturnsAsync(Status.Deleted);

        // Act
        var response = await _controller.DeleteProject(1);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(response);
    }

    [Fact]
    public async void DeleteProject_returns_NotFound_given_nonexistent_id()
    {
        // Arrange
        _projectRepository.Setup(m => m.Delete(10)).ReturnsAsync(Status.NotFound);

        // Act
        var response = await _controller.DeleteProject(10);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(response);
    }

    [Fact]
    public async void AddUserToProject_given_existing_id_adds_user_to_project_and_returns_NoContent()
    {
        // Arrange
        _projectRepository
            .Setup(m => m.Read(1))
            .ReturnsAsync(_project);
        
        _projectRepository
            .Setup(m => m.Update(1, new ProjectUpdateDTO()))
            .ReturnsAsync(Status.Updated);

        // Act
        var response = await _controller.AddUserToProject(1);

        // Assert
        Assert.IsType<BadRequestObjectResult>(response);
    }

    [Fact]
    public async void AddUserToProject_given_non_existing_id_returns_not_found()
    {
        // Arrange
        _projectRepository.Setup(m => m.Read(1)).Returns(Task.FromResult<ProjectDetailsDTO>(null));
        _projectRepository.Setup(m => m.Update(1, new ProjectUpdateDTO())).ReturnsAsync(Status.NotFound);

        // Act
        var response = await _controller.AddUserToProject(1);

        // Assert
        Assert.IsType<NotFoundObjectResult>(response);
    }

    [Fact]
    public async void RemoveUserFromProject_given_existing_id_removes_user_from_project_and_returns_NoContent()
    {
        // Arrange
        _projectRepository
            .Setup(m => m.Read(1))
            .ReturnsAsync(_project);
        
        _projectRepository
            .Setup(m => m.Update(1, new ProjectUpdateDTO()))
            .ReturnsAsync(Status.Updated);
        
        

        // Act
        var response = await _controller.RemoveUserFromProject(1);

        // Assert
        Assert.IsType<ConflictObjectResult>(response);
    }

    [Fact]
    public async void RemoveUserFromProject_given_non_existing_id_returns_not_found()
    {
        // Arrange
        _projectRepository.Setup(m => m.Read(1)).Returns(Task.FromResult<ProjectDetailsDTO>(null));
        _projectRepository.Setup(m => m.Update(1, new ProjectUpdateDTO())).ReturnsAsync(Status.NotFound);

        // Act
        var response = await _controller.RemoveUserFromProject(1);

        // Assert
        Assert.IsType<NotFoundObjectResult>(response);
    }
}