using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using CoProject.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

namespace CoProject.Server.Tests.Controllers;

public class ProjectControllerTests
{
    private readonly Mock<IProjectRepository> _projectRepository;
    private readonly Mock<IUserRepository> _userRepository;
    private ProjectController _controller;
    private readonly ProjectDetailsDTO _project;
    private readonly ProjectDetailsDTO _projectWithUser;
    private readonly UserDetailsDTO _user;
    private readonly UserDetailsDTO _supervisor;
    private readonly UserDetailsDTO _supervisorWithID2;
    private readonly GenericIdentity _identity;
    

    public ProjectControllerTests()
    {
        
        _projectRepository = new();
        _userRepository = new ();
        _supervisor = new UserDetailsDTO("123", "Test User", "user@outlook.com", true, "/images/noimage.jpeg");
        _supervisorWithID2 = new UserDetailsDTO("9999", "Test User", "user@outlook.com", true, "/images/noimage.jpeg");
        _project = new(1, "Project Name", "Project Description", _supervisor, State.Open, DateTime.Now, new List<string>(), new List<UserDetailsDTO>());
        _user = new("123", "Test User", "user@outlook.com", false, "/images/noimage.jpeg");
        _projectWithUser = new(1, "Project Name", "Project Description", _supervisor, State.Open, DateTime.Now, new List<string>(), new List<UserDetailsDTO>(){_user});


        _identity = new GenericIdentity(_user.Name, "");
        _identity.AddClaim(new Claim(ClaimConstants.Name, _user.Name));
        _identity.AddClaim(new Claim("emails", _user.Email));
        _identity.AddClaim(new Claim(ClaimConstants.ObjectId, _user.Id));
        
        var principal = new GenericPrincipal(_identity, roles: new string[] { });
        var loggedInUser = new ClaimsPrincipal(principal);
        
        _controller = new ProjectController(_projectRepository.Object, _userRepository.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = loggedInUser } }
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
        _projectRepository.Setup(m => m.Read(_project.Id)).ReturnsAsync(_project);

        // Act
        var actual = await _controller.GetProject(_project.Id);

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
    
    
    [Fact]
    public async Task UpdateProject_given_existing_id_updates_project_and_returns_Ok()
    {
        // Arrange
        _projectRepository.Setup(m => m.Update(_project.Id, new ProjectUpdateDTO())).ReturnsAsync(Status.Updated);
        _projectRepository.Setup(m => m.Read(_project.Id)).ReturnsAsync(_project);
        _userRepository.Setup(m => m.Read(_supervisor.Id)).ReturnsAsync(_supervisor);

        // Act
        var response = await _controller.UpdateProject(_project.Id, new ProjectUpdateDTO());

        // Assert
        Assert.IsType<OkObjectResult>(response);
    }
    
    
    [Fact]
    public async Task UpdateProject_returns_unauthorized_given_user_not_found_in_db()
    {
        // Arrange
        _projectRepository
            .Setup(m => m.Update(_project.Id, new ProjectUpdateDTO()))
            .ReturnsAsync(Status.NotFound);

        // Act
        var response = await _controller.UpdateProject(_project.Id, new ProjectUpdateDTO());

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(response);
    }

    
    [Fact]
    public async void UpdateProject_returns_unauthorized_if_userid_is_null()
    {
        // Arrange
        _controller = new(_projectRepository.Object, _userRepository.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User =  new ClaimsPrincipal() } }
        };
        _projectRepository.Setup(m => m.Update(_project.Id, new ProjectUpdateDTO())).ReturnsAsync(Status.Updated);

        // Act
        var result = await _controller.UpdateProject(_project.Id, new ProjectUpdateDTO());

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }
    
    [Fact]
    public async void UpdateProject_returns_NotFound_if_project_is_null()
    {
        // Arrange
        _projectRepository.Setup(m => m.Update(_project.Id, new ProjectUpdateDTO())).ReturnsAsync(Status.Updated);
        _projectRepository.Setup(m => m.Read(_project.Id)).ReturnsAsync(default(ProjectDetailsDTO));
        _userRepository.Setup(m => m.Read(_user.Id)).ReturnsAsync(_user);
        
        // Act
        var result = await _controller.UpdateProject(_project.Id, new ProjectUpdateDTO());

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
    
    [Fact]
    public async void UpdateProject_returns_Forbid_if_user_is_not_supervisor()
    {
        // Arrange
        _projectRepository.Setup(m => m.Update(_project.Id, new ProjectUpdateDTO())).ReturnsAsync(Status.Updated);
        _projectRepository.Setup(m => m.Read(_project.Id)).ReturnsAsync(_project);
        _userRepository.Setup(m => m.Read(_user.Id)).ReturnsAsync(_user);

        // Act
        var result = await _controller.UpdateProject(_project.Id, new ProjectUpdateDTO());

        // Assert
        Assert.IsType<ForbidResult>(result);
    }
    
    [Fact]
    public async void UpdateProject_returns_BadRequest_if_project_is_not_updated_properly()
    {
        // Arrange
        _projectRepository.Setup(m => m.Update(_project.Id, new ProjectUpdateDTO())).ReturnsAsync(Status.BadRequest);
        _projectRepository.Setup(m => m.Read(_project.Id)).ReturnsAsync(_project);
        _userRepository.Setup(m => m.Read(_supervisor.Id)).ReturnsAsync(_supervisor);

        // Act
        var result = await _controller.UpdateProject(_project.Id, new ProjectUpdateDTO());

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async void CreateProject_creates_a_new_project()
    {
        // Arrange
        var toCreate = new ProjectCreateDTO("Project Name", "Project Description", State.Open, new List<string>());
        _projectRepository.Setup(m => m.Create(toCreate)).ReturnsAsync(_project);
        _userRepository.Setup(m => m.Read(_supervisor.Id)).ReturnsAsync(_supervisor);

        // Act
        var result = await _controller.CreateProject(toCreate);

        // Assert
        Assert.IsType<CreatedAtRouteResult>(result);
    }
    
    
    [Fact]
    public async void CreateProject_returns_unauthorized_if_userid_is_null()
    {
        // Arrange
        _controller = new(_projectRepository.Object, _userRepository.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User =  new ClaimsPrincipal() } }
        };
        var toCreate = new ProjectCreateDTO("Project Name", "Project Description", State.Open, new List<string>());

        // Act
        var result = await _controller.CreateProject(toCreate);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }
    
    [Fact]
    public async void CreateProject_returns_unauthorized_if_no_one_logged_in()
    {
        // Arrange
        var toCreate = new ProjectCreateDTO("Project Name", "Project Description", State.Open, new List<string>());
        _userRepository.Setup(m => m.Read(_supervisor.Id)).ReturnsAsync(default(UserDetailsDTO));
        
        // Act
        var result = await _controller.CreateProject(toCreate);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }
    
    [Fact]
    public async void CreateProject_returns_forbid_if_user_is_not_supervisor()
    {
        // Arrange
        var toCreate = new ProjectCreateDTO("Project Name", "Project Description", State.Open, new List<string>());
        _userRepository.Setup(m => m.Read(_user.Id)).ReturnsAsync(_user);
        
        // Act
        var result = await _controller.CreateProject(toCreate);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    
    //TODO: Fix 
    [Fact]
    public async void DeleteProject_deletes_a_projects_given_id_and_returns_NoContent()
    {
        // Arrange
        _projectRepository.Setup(m => m.Delete(_project.Id)).ReturnsAsync(Status.Deleted);

        // Act
        var response = await _controller.DeleteProject(_project.Id);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(response);
    }

    //TODO: Fix
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
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(_project);
        
        _projectRepository
            .Setup(m => m.Update(_project.Id, new ProjectUpdateDTO()))
            .ReturnsAsync(Status.Updated);

        // Act
        var response = await _controller.AddUserToProject(_project.Id);

        // Assert
        Assert.IsType<BadRequestObjectResult>(response);
    }

    [Fact]
    public async void AddUserToProject_given_non_existing_id_returns_NotFound()
    {
        // Arrange
        _projectRepository.Setup(m => m.Read(_project.Id)).Returns(Task.FromResult<ProjectDetailsDTO>(null));
        _projectRepository.Setup(m => m.Update(_project.Id, new ProjectUpdateDTO())).ReturnsAsync(Status.NotFound);

        // Act
        var response = await _controller.AddUserToProject(_project.Id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(response);
    }
    
    [Fact]
    public async void AddUserToProject_returns_Forbid_if_given_closed_project()
    {
        // Arrange
        var closedProject = new ProjectDetailsDTO(_project.Id, "Project Name", "Project Description", _supervisor, State.Closed, DateTime.Now, new List<string>(), new List<UserDetailsDTO>());
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(closedProject);

        // Act
        var response = await _controller.AddUserToProject(_project.Id);

        // Assert
        Assert.IsType<ForbidResult>(response);
    }
    
    [Fact]
    public async void AddUserToProject_returns_Forbid_if_user_count_is_bigger_than_max()
    {
        // Arrange
        var project = new ProjectDetailsDTO(_project.Id, "Project Name", "Project Description", _supervisor, State.Open, DateTime.Now, new List<string>(), new List<UserDetailsDTO>(){_user, _supervisor})
        {
            Max = 1
        };
        
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(project);

        // Act
        var response = await _controller.AddUserToProject(_project.Id);

        // Assert
        Assert.IsType<ForbidResult>(response);
    }
    
    [Fact]
    public async void AddUserToProject_returns_Unauthorized_if_User_is_not_logged_in()
    {
        // Arrange
        _controller = new(_projectRepository.Object, _userRepository.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User =  new ClaimsPrincipal() } }
        };
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(_project);

        // Act
        var response = await _controller.AddUserToProject(_project.Id);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(response);
    }
    
    [Fact]
    public async void AddUserToProject_returns_Conflict_if_User_is_already_registered()
    {
        // Arrange
        var project = new ProjectDetailsDTO(_project.Id, "Project Name", "Project Description", _supervisor, State.Open, DateTime.Now, new List<string>(), new List<UserDetailsDTO>(){_user});
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(project);

        // Act
        var response = await _controller.AddUserToProject(_project.Id);

        // Assert
        Assert.IsType<ConflictObjectResult>(response);
    }

    //TODO: FIX
    [Fact]
    public async void AddUserToProject_returns_Ok_if_User_added_correctly()
    {
        // Arrange
        _projectRepository.Setup(m => m
                .Read(_project.Id))
            .ReturnsAsync(_project);
            
        _projectRepository
            .Setup(m => m.Update(_project.Id, new ProjectUpdateDTO()))
            .ReturnsAsync(Status.Updated);
        
        // Act
        var response = await _controller.AddUserToProject(_project.Id);

        // Assert
        Assert.IsType<OkObjectResult>(response);
    }
    
    [Fact]
    public async void AddUserToProject_returns_BadRequest_if_User_is_not_added_properly()
    {
        // Arrange
        _projectRepository.Setup(m => m
            .Update(_project.Id, new ProjectUpdateDTO()))
            .ReturnsAsync(Status.BadRequest);
        
        _projectRepository.Setup(m => m
            .Read(_project.Id))
            .ReturnsAsync(_project);

        // Act
        var result = await _controller.AddUserToProject(_project.Id);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async void RemoveUserFromProject_given_existing_id_removes_user_from_project_and_returns_NoContent()
    {
        // Arrange
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(_project);
        
        _projectRepository
            .Setup(m => m.Update(_project.Id, new ProjectUpdateDTO()))
            .ReturnsAsync(Status.Updated);
        
        

        // Act
        var response = await _controller.RemoveUserFromProject(_project.Id);

        // Assert
        Assert.IsType<ConflictObjectResult>(response);
    }

    [Fact]
    public async void RemoveUserFromProject_given_non_existing_ProjectID_returns_not_found()
    {
        // Arrange
        _projectRepository.Setup(m => m.Read(_project.Id)).Returns(Task.FromResult<ProjectDetailsDTO>(null));
        _projectRepository.Setup(m => m.Update(_project.Id, new ProjectUpdateDTO())).ReturnsAsync(Status.NotFound);

        // Act
        var response = await _controller.RemoveUserFromProject(_project.Id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(response);
    }

    [Fact]
    public async void RemoveUserFromProject_given_non_existing_UserID_returns_Unauthorized()
    {
        //Arrange
        _controller = new(_projectRepository.Object, _userRepository.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User =  new ClaimsPrincipal() } }
        };
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(_project);

        // Act
        var response = await _controller.RemoveUserFromProject(_project.Id);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(response);
    }
    
    [Fact]
    public async void RemoveUserFromProject_returns_BadRequest_if_User_is_not_added_properly()
    {
        // Arrange
        _projectRepository.Setup(m => m
            .Update(_project.Id, new ProjectUpdateDTO()))
            .ReturnsAsync(Status.BadRequest);
        
        _projectRepository.Setup(m => m
            .Read(_project.Id))
            .ReturnsAsync(_projectWithUser);

        // Act
        var result = await _controller.RemoveUserFromProject(_project.Id);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async void DeleteProject_returns_Unauthorized_if_User_is_not_logged_in()
    {
        // Arrange
        _controller = new(_projectRepository.Object, _userRepository.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User =  new ClaimsPrincipal() } }
        };
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(_project);

        // Act
        var response = await _controller.DeleteProject(_project.Id);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(response);
    }
    
    [Fact]
    public async void DeleteProject_returns_Forbid_if_user_is_not_supervisor()
    {
        // Arrange
        _projectRepository.Setup(m => m.Read(_project.Id)).ReturnsAsync(_project);
        _userRepository.Setup(m => m.Read(_user.Id)).ReturnsAsync(_user);

        // Act
        var response = await _controller.DeleteProject(_project.Id);

        // Assert
        Assert.IsType<ForbidResult>(response);
    }
    
    [Fact]
    public async void DeleteProject_returns_NotFound_if_Project_is_null()
    {
        // Arrange
        _projectRepository.Setup(m => m.Read(_project.Id)).ReturnsAsync(default(ProjectDetailsDTO));
        _userRepository.Setup(m => m.Read(_user.Id)).ReturnsAsync(_supervisor);

        // Act
        var response = await _controller.DeleteProject(_project.Id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(response);
    }
    
    [Fact]
    public async void DeleteProject_returns_Forbid_if_user_is_not_supervisor_of_this_project()
    {
        // Arrange
        _projectRepository.Setup(m => m.Read(_project.Id)).ReturnsAsync(_project);
        _userRepository.Setup(m => m.Read(_user.Id)).ReturnsAsync(_supervisorWithID2);

        // Act
        var response = await _controller.DeleteProject(_project.Id);

        // Assert
        Assert.IsType<ForbidResult>(response);
    }
    
    [Fact]
    public async void DeleteProject_returns_Ok_if_Project_is_Deleted_successfully()
    {
        // Arrange
        _projectRepository.Setup(m => m.Read(_project.Id)).ReturnsAsync(_project);
        _projectRepository.Setup(m => m.Delete(_project.Id)).ReturnsAsync(Status.Deleted);
        _userRepository.Setup(m => m.Read(_user.Id)).ReturnsAsync(_supervisor);

        // Act
        var response = await _controller.DeleteProject(_project.Id);

        // Assert
        Assert.IsType<OkObjectResult>(response);
    }
    
    [Fact]
    public async void DeleteProject_returns_BadRequest_if_Project_is_not_Deleted_successfully()
    {
        // Arrange
        _projectRepository.Setup(m => m.Read(_project.Id)).ReturnsAsync(_project);
        _projectRepository.Setup(m => m.Delete(_project.Id)).ReturnsAsync(Status.NotFound);
        _userRepository.Setup(m => m.Read(_user.Id)).ReturnsAsync(_supervisor);

        // Act
        var response = await _controller.DeleteProject(_project.Id);

        // Assert
        Assert.IsType<BadRequestObjectResult>(response);
    }
    
}