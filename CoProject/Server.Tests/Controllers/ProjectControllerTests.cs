using System.Linq;

namespace CoProject.Server.Tests.Controllers;

public class ProjectControllerTests : DefaultTests
{
    private readonly ProjectDetailsDTO _project;
    private readonly Mock<IProjectRepository> _projectRepository;
    private readonly UserDetailsDTO _supervisor;
    private readonly Mock<IUserRepository> _userRepository;
    private ProjectController _controller;

    public ProjectControllerTests()
    {
        _projectRepository = new();
        _userRepository = new();
        _supervisor = new("123", "Test User", "user@outlook.com", true, "/images/noimage.jpeg");
        _project = new(1, "Project Name", "Project Description", _supervisor, State.Open, DateTime.Now, new List<string>(), new List<UserDetailsDTO>());

        _controller = new(_projectRepository.Object, _userRepository.Object)
        {
            ControllerContext = ControllerContext
        };
    }

    [Fact]
    public async void GetProjects_returns_all_projects()
    {
        // Arrange
        var expected = Array.Empty<ProjectDetailsDTO>();
        _projectRepository
            .Setup(m => m.ReadAll())
            .ReturnsAsync(expected);

        // Act
        var actual = await _controller.GetProjects();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetProject_returns_project_given_id()
    {
        // Arrange
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(_project);

        // Act
        var actual = await _controller.GetProject(_project.Id);

        // Assert
        Assert.Equal(_project, actual.Value);
    }


    [Fact]
    public async Task GetProject_returns_NotFoundObject_given_nonexistent_id()
    {
        // Arrange
        _projectRepository
            .Setup(m => m.Read(100))
            .ReturnsAsync(default(ProjectDetailsDTO));

        // Act
        var response = await _controller.GetProject(100);

        // Assert
        Assert.IsType<NotFoundObjectResult>(response.Result);
    }


    [Fact]
    public async Task UpdateProject_given_existing_id_updates_project_and_returns_Ok()
    {
        // Arrange
        _projectRepository
            .Setup(m => m.Update(_project.Id, new()))
            .ReturnsAsync(Status.Updated);
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(_project);
        _userRepository
            .Setup(m => m.Read(_supervisor.Id))
            .ReturnsAsync(_supervisor);

        // Act
        var response = await _controller.UpdateProject(_project.Id, new());

        // Assert
        Assert.IsType<OkObjectResult>(response);
    }


    [Fact]
    public async Task UpdateProject_returns_unauthorized_given_user_not_found_in_db()
    {
        // Arrange
        _projectRepository
            .Setup(m => m.Update(_project.Id, new()))
            .ReturnsAsync(Status.NotFound);

        // Act
        var response = await _controller.UpdateProject(_project.Id, new());

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(response);
    }


    [Fact]
    public async void UpdateProject_returns_unauthorized_if_userid_is_null()
    {
        // Arrange
        _controller = new(_projectRepository.Object, _userRepository.Object)
        {
            ControllerContext = new() {HttpContext = new DefaultHttpContext {User = new()}}
        };
        _projectRepository
            .Setup(m => m.Update(_project.Id, new()))
            .ReturnsAsync(Status.Updated);

        // Act
        var result = await _controller.UpdateProject(_project.Id, new());

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async void UpdateProject_returns_NotFound_if_project_is_null()
    {
        // Arrange
        _projectRepository
            .Setup(m => m.Update(_project.Id, new()))
            .ReturnsAsync(Status.Updated);
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(default(ProjectDetailsDTO));
        _userRepository
            .Setup(m => m.Read(User.Id))
            .ReturnsAsync(User);

        // Act
        var result = await _controller.UpdateProject(_project.Id, new());

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async void UpdateProject_returns_Forbid_if_user_is_not_supervisor()
    {
        // Arrange
        _projectRepository
            .Setup(m => m.Update(_project.Id, new()))
            .ReturnsAsync(Status.Updated);
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(_project);
        _userRepository
            .Setup(m => m.Read(User.Id))
            .ReturnsAsync(User);

        // Act
        var result = await _controller.UpdateProject(_project.Id, new());

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async void UpdateProject_returns_BadRequest_if_project_is_not_updated_properly()
    {
        // Arrange
        _projectRepository
            .Setup(m => m.Update(_project.Id, new()))
            .ReturnsAsync(Status.BadRequest);
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(_project);
        _userRepository
            .Setup(m => m.Read(_supervisor.Id))
            .ReturnsAsync(_supervisor);

        // Act
        var result = await _controller.UpdateProject(_project.Id, new());

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async void CreateProject_creates_a_new_project()
    {
        // Arrange
        var toCreate = new ProjectCreateDTO("Project Name", "Project Description", State.Open, new List<string>());
        _projectRepository
            .Setup(m => m.Create(toCreate))
            .ReturnsAsync(_project);
        _userRepository
            .Setup(m => m.Read(_supervisor.Id))
            .ReturnsAsync(_supervisor);

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
            ControllerContext = new() {HttpContext = new DefaultHttpContext {User = new()}}
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
        _userRepository
            .Setup(m => m.Read(_supervisor.Id))
            .ReturnsAsync(default(UserDetailsDTO));

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

        _userRepository
            .Setup(m => m.Read(User.Id))
            .ReturnsAsync(User);

        // Act
        var result = await _controller.CreateProject(toCreate);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }
    
    [Fact]
    public async void AddUserToProject_given_existing_id_adds_user_to_project_and_returns_NoContent()
    {
        // Arrange
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(_project);

        _projectRepository
            .Setup(m => m.Update(_project.Id, new()))
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
        _projectRepository
            .Setup(m => m.Read(1))
            .ReturnsAsync(default(ProjectDetailsDTO));
        _projectRepository
            .Setup(m => m.Update(1, new()))
            .ReturnsAsync(Status.NotFound);

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
        var project = new ProjectDetailsDTO(_project.Id, "Project Name", "Project Description", _supervisor, State.Open, DateTime.Now, new List<string>(), new List<UserDetailsDTO> {User, _supervisor})
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
            ControllerContext = new() {HttpContext = new DefaultHttpContext {User = new()}}
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
        var project = new ProjectDetailsDTO(_project.Id, "Project Name", "Project Description", _supervisor, State.Open, DateTime.Now, new List<string>(), new List<UserDetailsDTO> {User});

        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(project);

        // Act
        var response = await _controller.AddUserToProject(_project.Id);

        // Assert
        Assert.IsType<ConflictObjectResult>(response);
    }

    [Fact]
    public async void AddUserToProject_returns_Ok_if_User_added_correctly()
    {
        // Arrange
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(_project);

        _projectRepository
            .Setup(m => m.Update(_project.Id, It.IsAny<ProjectUpdateDTO>()))
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
        _projectRepository
            .Setup(m => m.Update(_project.Id, new()))
            .ReturnsAsync(Status.BadRequest);

        _projectRepository
            .Setup(m => m.Read(_project.Id))
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
            .Setup(m => m.Update(_project.Id, new()))
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
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(default(ProjectDetailsDTO));
        _projectRepository
            .Setup(m => m.Update(_project.Id, new()))
            .ReturnsAsync(Status.NotFound);


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
            ControllerContext = new() {HttpContext = new DefaultHttpContext {User = new()}}
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
    public async void RemoveUserFromProject_returns_Ok_if_User_removed_correctly()
    {
        // Arrange
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(_project);

        _projectRepository
            .Setup(m => m.Update(_project.Id, It.IsAny<ProjectUpdateDTO>()))
            .ReturnsAsync(Status.Updated);

        // Act
        var response = await _controller.AddUserToProject(_project.Id);

        // Assert
        Assert.IsType<OkObjectResult>(response);
    }

    [Fact]
    public async void RemoveUserFromProject_returns_BadRequest_if_User_is_not_added_properly()
    {
        // Arrange
        _projectRepository
            .Setup(m => m.Update(_project.Id, new()))
            .ReturnsAsync(Status.BadRequest);

        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(new ProjectDetailsDTO(1, "Project Name", "Project Description", _supervisor, State.Open, DateTime.Now, new List<string>(), new List<UserDetailsDTO> {User}));

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
            ControllerContext = new() {HttpContext = new DefaultHttpContext {User = new()}}
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
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(_project);
        _userRepository
            .Setup(m => m.Read(User.Id))
            .ReturnsAsync(User);

        // Act
        var response = await _controller.DeleteProject(_project.Id);

        // Assert
        Assert.IsType<ForbidResult>(response);
    }

    [Fact]
    public async void DeleteProject_returns_NotFound_if_Project_is_null()
    {
        // Arrange
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(default(ProjectDetailsDTO));
        _userRepository
            .Setup(m => m.Read(User.Id))
            .ReturnsAsync(_supervisor);

        // Act
        var response = await _controller.DeleteProject(_project.Id);

        // Assert
        Assert.IsType<NotFoundObjectResult>(response);
    }

    [Fact]
    public async void DeleteProject_returns_Forbid_if_user_is_not_supervisor_of_this_project()
    {
        // Arrange
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(_project);
        _userRepository
            .Setup(m => m.Read(User.Id))
            .ReturnsAsync(new UserDetailsDTO("9999", "Test User", "user@outlook.com", true, "/images/noimage.jpeg"));

        // Act
        var response = await _controller.DeleteProject(_project.Id);

        // Assert
        Assert.IsType<ForbidResult>(response);
    }

    [Fact]
    public async void DeleteProject_returns_Ok_if_Project_is_Deleted_successfully()
    {
        // Arrange
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(_project);
        _projectRepository
            .Setup(m => m.Delete(_project.Id))
            .ReturnsAsync(Status.Deleted);
        _userRepository
            .Setup(m => m.Read(User.Id))
            .ReturnsAsync(_supervisor);

        // Act
        var response = await _controller.DeleteProject(_project.Id);

        // Assert
        Assert.IsType<OkObjectResult>(response);
    }

    [Fact]
    public async void DeleteProject_returns_BadRequest_if_Project_is_not_Deleted_successfully()
    {
        // Arrange
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(_project);
        _projectRepository
            .Setup(m => m.Delete(_project.Id))
            .ReturnsAsync(Status.NotFound);
        _userRepository
            .Setup(m => m.Read(User.Id))
            .ReturnsAsync(_supervisor);

        // Act
        var response = await _controller.DeleteProject(_project.Id);

        // Assert
        Assert.IsType<BadRequestObjectResult>(response);
    }
    
    [Fact]
    public async void DeleteProject_returns_Unauthorized_if_User_is_null()
    {
        // Arrange
        _projectRepository
            .Setup(m => m.Read(_project.Id))
            .ReturnsAsync(_project);

        // Act
        var response = await _controller.DeleteProject(_project.Id);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(response);
    }
}