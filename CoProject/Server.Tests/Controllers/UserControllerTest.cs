using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using CoProject.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoProject.Server.Tests.Controllers;

public class UserControllerTest
{

    private readonly Mock<IUserRepository> _repository;
    private UserController _controller;
    private readonly UserDetailsDTO _user;
    private GenericIdentity _identity;

    public UserControllerTest()
    {
        _repository = new();

        _user = new UserDetailsDTO("12345", "Mikkel", "milb@itu.dk", false);

        _identity = new GenericIdentity(_user.Name, "");
        _identity.AddClaim(new Claim("name", _user.Name));
        _identity.AddClaim(new Claim("emails", _user.Email));

        _identity.AddClaim(new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", _user.Id));


        var principal = new GenericPrincipal(_identity, roles: new string[] { });
        var loggedInUser = new ClaimsPrincipal(principal);


        _controller = new UserController(_repository.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = loggedInUser } }
        };
    }


    [Fact]
    public async void GetUser_returns_logged_in_user()
    {
        // Arrange
        _repository.Setup(m => m.Read(_user.Id)).ReturnsAsync(_user);

        // act
        var actual = await _controller.GetUser();

        // assert
        Assert.Equal(_user, actual.Value);


    }

    [Fact]
    public async void GetUser_returns_not_found_in_token()
    {
        // Arrange
        _identity = new GenericIdentity(_user.Name, "");
        var principal = new GenericPrincipal(_identity, roles: new string[] { });
        var loggedInUser = new ClaimsPrincipal(principal);


        _controller = new UserController(_repository.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = loggedInUser } }
        };

        // act
        var actual = await _controller.GetUser();

        // assert
        Assert.IsType<NotFoundResult>(actual.Result);

    }

    [Fact]
    public async void GetUser_returns_not_found_in_database()
    {
        // Arrange
        _repository.Setup(m => m.Read(_user.Id)).ReturnsAsync(default(UserDetailsDTO));

        // act
        var actual = await _controller.GetUser();

        // assert
        Assert.IsType<NotFoundResult>(actual.Result);

    }

    [Fact]
    public async void getProjectByUser_returns_found_projects()
    {
        // Arrange
        var projects = new List<ProjectDetailsDTO>();
        projects.Add(new(1, "Project Name", "Project Description", _user, State.Open, DateTime.Now, new List<string>(), new List<UserDetailsDTO>()));
        _repository.Setup(m => m.ReadAllByUser(_user.Id)).ReturnsAsync(projects);

        // Act
        var actual = await _controller.GetProjectsByUser();

        // assert
        Assert.Equal(projects, actual);
    }

    [Fact]
    public async void getProjectsByUser_returns_empty()
    {
        // Arrange
        _identity = new GenericIdentity(_user.Name, "");
        var principal = new GenericPrincipal(_identity, roles: new string[] { });
        var loggedInUser = new ClaimsPrincipal(principal);


        _controller = new UserController(_repository.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = loggedInUser } }
        };

        // Act
        var actual = await _controller.GetProjectsByUser();

        Assert.Equal(new List<ProjectDetailsDTO>(), actual);
    }

    [Fact]
    public async void SignupUser_returns_not_found()
    {
        // No arrange needed
        _identity = new GenericIdentity(_user.Name, "");

        _identity.AddClaim(new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", _user.Id));


        var principal = new GenericPrincipal(_identity, roles: new string[] { });
        var loggedInUser = new ClaimsPrincipal(principal);


        _controller = new UserController(_repository.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = loggedInUser } }
        };

        // act
        var actual = await _controller.SignupUser();

        // assert
        Assert.IsType<NotFoundResult>(actual.Result);

    }

    [Fact]
    public async void SignupUser_returns_no_content()
    {
        // arrange
        _repository.Setup(m => m.Read(_user.Id)).ReturnsAsync(_user);

        // act
        var actual = await _controller.SignupUser();

        // assert
        Assert.IsType<NoContentResult>(actual.Result);

    }

    [Fact]
    public async void SignupUser_returns_created()
    {
        // arrange
        _repository.Setup(m => m.Read(_user.Id)).ReturnsAsync(default(UserDetailsDTO));
        var userCreate = new UserCreateDTO(_user.Id, _user.Name, _user.Email, _user.Supervisor);
        _repository.Setup(m => m.Create(userCreate)).ReturnsAsync(_user);

        // act
        var actual = await _controller.SignupUser();

        // assert
        //Assert.Equal(_user, actual.Value);
        Assert.IsType<CreatedAtActionResult>(actual.Result);


    }

    [Fact]
    public async void UpdateUser_returns_not_found()
    {
        // Arrange
        _identity = new GenericIdentity(_user.Name, "");


        var principal = new GenericPrincipal(_identity, roles: new string[] { });
        var loggedInUser = new ClaimsPrincipal(principal);


        _controller = new UserController(_repository.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = loggedInUser } }
        };

        var updatedUser = new UserUpdateDTO("Jens", "jens@itu.dk");


        // act
        var actual = await _controller.UpdateUser(updatedUser);

        // assert
        Assert.IsType<NotFoundResult>(actual.Result);
    }

    [Fact]
    public async void UpdateUser_returns_no_content()
    {
        // Arrange
        var updatedUser = new UserUpdateDTO("Jens", "jens@itu.dk");
        _repository.Setup(m => m.Update(_user.Id, updatedUser)).ReturnsAsync(Status.Updated);

        // act
        var actual = await _controller.UpdateUser(updatedUser);

        // assert
        Assert.IsType<NoContentResult>(actual.Result);
    }
    /*
    
    
    [Fact]
    public async void GetUser_returns_user_given_id()
    {
        //Arrange
        var project = new UserDetailsDTO(1, "Example User", "Example User Name", "user@user.dk");
        repository.Setup(m => m.Read(1)).ReturnsAsync(project);

        //Act
        var actual = await controller.GetUser(1);
        
        //Assert
        Assert.Equal(project, actual.Value);
    }

    [Fact]
    public async void GetUser_returns_notfound_given_nonexistent_id()
    {
        //Arrange
        repository.Setup(m => m.Read(100)).ReturnsAsync(default(UserDetailsDTO));
        
        //Act
        var response = await controller.GetUser(100);
        
        //Assert
        Assert.IsType<NotFoundResult>(response.Result);
    }

    [Fact]
    public async void GetProjectsByUser_returns_list_of_projects_by_user()
    {
        //Arrange
        var projects = new List<ProjectDetailsDTO>
        {
            new(1, "Project Name", "Project Description", 1, State.Open, DateTime.Now, new List<string>(), new List<UserDetailsDTO>())
        };
        repository.Setup(m => m.ReadAllByUser(1)).ReturnsAsync(projects);

        var response = await controller.GetProjectsByUser(1);
        
        Assert.Equal(projects, response);

    }
    
    [Fact]
    public async void CreateUser_creates_a_new_user()
    {
        //Arrange
        var toCreate = new UserCreateDTO(
            2, "Wee", "WeeButNormalized", "wee@wee.dk", "wee@wee.dk", "12345678",
            "N/A", "N/A", "N/A", true, true, false, true, false, 0, new List<Project>()
        );
        var project = new UserDetailsDTO(1, "Example User", "Example User Name", "user@user.dk");
        repository.Setup(m => m.Create(toCreate)).ReturnsAsync(project);
        
        //Act
        var result = await controller.CreateUser(toCreate) as CreatedAtRouteResult;
        
        //Assert
        Assert.Equal(project, result?.Value);
        Assert.Equal("GetUser", result?.RouteName);
        Assert.Equal(KeyValuePair.Create("Id", (object?)1), result?.RouteValues?.Single());
    }
    
    [Fact]
    public async void UpdateUser_given_existing_id_updates_user_and_returns_NoContent()
    {
        //Arrange
        var user = new UserUpdateDTO("Example User", "user@user.dk");
        repository.Setup(m => m.Update(1, user)).ReturnsAsync(Status.Updated);
        
        //Act
        var response = await controller.UpdateUser(1, user);

        //Assert
        Assert.IsType<NoContentResult>(response);
    }
    
    [Fact]
    public async void UpdateUser_given_nonexistent_id_returns_NotFound()
    {
        //Arrange
        var user = new UserUpdateDTO("Example User", "user@user.dk");
        repository.Setup(m => m.Update(1, user)).ReturnsAsync(Status.NotFound);
        
        //Act
        var response = await controller.UpdateUser(1, user);

        //Assert
        Assert.IsType<NotFoundResult>(response);
    }
    
    [Fact]
    public async void DeleteUser_deletes_a_user_given_id_and_returns_NoContent()
    {
        // Arrange
        repository.Setup(m => m.Delete(1)).ReturnsAsync(Status.Deleted);
        
        // Act
        var response = await controller.DeleteUser(1);

        // Assert
        Assert.IsType<NoContentResult>(response);
    }

    [Fact]
    public async void DeleteUser_returns_NotFound_given_nonexistent_id()
    {
        // Arrange
        repository.Setup(m => m.Delete(100)).ReturnsAsync(Status.NotFound);
        
        // Act
        var response = await controller.DeleteUser(100);

        // Assert
        Assert.IsType<NotFoundResult>(response);
    }
    */
}