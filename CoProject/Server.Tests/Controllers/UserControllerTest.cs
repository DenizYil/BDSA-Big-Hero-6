using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using CoProject.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

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
        _identity.AddClaim(new Claim(ClaimConstants.Name, _user.Name));
        _identity.AddClaim(new Claim("emails", _user.Email));

        _identity.AddClaim(new Claim(ClaimConstants.ObjectId, _user.Id));


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

        _identity.AddClaim(new Claim(ClaimConstants.ObjectId, _user.Id));


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
}