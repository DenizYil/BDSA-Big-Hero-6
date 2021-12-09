using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using CoProject.Shared;
using Microsoft.AspNetCore.Hosting;
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
    private readonly Mock<IWebHostEnvironment> _env;
    private readonly Mock<FileStream> _file;
    private readonly Mock<System.IO.Abstractions.FileBase> _fileWriter;

    public UserControllerTest()
    {
        
        // SETUP
        _env = new();
        _fileWriter = new();
        _file = new();
        _env.Setup(m => m.WebRootPath).Returns("../../../");
        _repository = new();

        _user = new UserDetailsDTO("12345", "Mikkel", "milb@itu.dk", false, "/images/noimage.jpeg");

        _identity = new GenericIdentity(_user.Name, "");
        _identity.AddClaim(new Claim(ClaimConstants.Name, _user.Name));
        _identity.AddClaim(new Claim("emails", _user.Email));
        _identity.AddClaim(new Claim(ClaimConstants.ObjectId, _user.Id));


        var principal = new GenericPrincipal(_identity, roles: new string[] { });
        var loggedInUser = new ClaimsPrincipal(principal);


        _controller = new UserController(_repository.Object, _env.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = loggedInUser } }
        };
    }
    [Fact]
    public async void GetUser_returns_no_token_found()
    {
        // Arrange
        // There is no token


        _controller = new UserController(_repository.Object, _env.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User =  new ClaimsPrincipal() } }
        };

        // act
        var actual = await _controller.GetUser();

        // assert
        Assert.IsType<UnauthorizedObjectResult>(actual.Result);

    }

    [Fact]
    public async void GetUser_returns_not_found_in_database()
    {
        // Arrange
        // User is logged in but not found in database
        _repository.Setup(m => m.Read(_user.Id)).ReturnsAsync(default(UserDetailsDTO));

        // act
        var actual = await _controller.GetUser();

        // assert
        Assert.IsType<UnauthorizedObjectResult>(actual.Result);

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
    public async void getProjectsByUser_returns_found_projects()
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


        _controller = new UserController(_repository.Object, _env.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() } }
        };

        // Act
        var actual = await _controller.GetProjectsByUser();

        Assert.Equal(new List<ProjectDetailsDTO>(), actual);
    }

    [Fact]
    public async void SignupUser_returns_unauthorized_given_no_token()
    {
        // Arrange
        _controller = new UserController(_repository.Object, _env.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() } }
        };

        // act
        var actual = await _controller.SignupUser();

        // assert
        Assert.IsType<UnauthorizedObjectResult>(actual.Result);

    }

    [Fact]
    public async void SignupUser_returns_created_user_given_not_found_in_db()
    {
        // arrange
        _repository.Setup(m => m.Read(_user.Id)).ReturnsAsync(default(UserDetailsDTO));
        var userCreate = new UserCreateDTO(_user.Id, _user.Name, _user.Email, _user.Supervisor);
        _repository.Setup(m => m.Create(userCreate)).ReturnsAsync(_user);

        // act
        var response = await _controller.SignupUser();

        // assert
        var okObjectResult = response.Result as OkObjectResult;
        Assert.NotNull(okObjectResult);
        var actual = okObjectResult.Value as UserDetailsDTO;
        Assert.Equal(_user, actual);

    }

    [Fact]
    public async void SignupUser_returns_already_found_user()
    {
        // arrange
        _repository.Setup(m => m.Read(_user.Id)).ReturnsAsync(_user);

        // act
        var response = await _controller.SignupUser();

        // assert
        
        var okObjectResult = response.Result as OkObjectResult;
        Assert.NotNull(okObjectResult);
        var actual = okObjectResult.Value as UserDetailsDTO;
        Assert.Equal(_user, actual);


    }

    [Fact]
    public async void UpdateUser_returns_unauthorized_given_no_token()
    {
        // Arrange


        _controller = new UserController(_repository.Object, _env.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() } }
        };
        var updateBody = new UpdateUserBody();


        // act
        var actual = await _controller.UpdateUser(updateBody);

        // assert
        Assert.IsType<UnauthorizedObjectResult>(actual.Result);
    }

    [Fact]
    public async void UpdateUser_returns_badrequest_given_no_jpg_or_png()
    {
        // Arrange
        // Uesr is logged in
        var updateFile = new UploadedFile() {FileContent = new byte[200], FileName = "image.pdf"};
        var updateBody = new UpdateUserBody() {file = updateFile};
        
        // act
        var actual = await _controller.UpdateUser(updateBody);

        // assert
        Assert.IsType<BadRequestObjectResult>(actual.Result);
    }

    [Fact]
    public async void UpdateUser_returns_unauthorized_given_user_not_found_in_db()
    {
        // Arrange
        // Uesr is logged in
        var updateFile = new UploadedFile() {FileContent = new byte[200], FileName = "image.jpg"};
        var updateBody = new UpdateUserBody() {file = updateFile};
        
        // act
        var actual = await _controller.UpdateUser(updateBody);

        // assert
        Assert.IsType<UnauthorizedObjectResult>(actual.Result);
    }
    [Fact]
    public async void UpdateUser_returns_ok_given_profile_is_updated()
    {
        // Arrange
        // User is logged in here...
        var updatedUser = new UserUpdateDTO("Jens", "jens@itu.dk");
        var updateFile = new UploadedFile() {FileContent = new byte[200], FileName = "image.jpg"};
        var updateBody = new UpdateUserBody() {file = updateFile, updatedUser = updatedUser};
        var userImagePath = $"/userimages/{_user.Id}_{Guid.NewGuid()}.jpg";
        _repository.Setup(m => m.Read(_user.Id)).ReturnsAsync(_user);
        _fileWriter.Setup(m => m.Create(userImagePath)).Returns(() => new FileStream("", FileMode.Create));
        _file.Setup(m => m.Write(updateFile.FileContent,  0, updateFile.FileContent.Length)).Verifiable();
        _repository.Setup(m => m.Update(_user.Id, updatedUser)).ReturnsAsync(Status.Updated);

        // act
        var actual = await _controller.UpdateUser(updateBody);

        // assert
        Assert.IsType<OkObjectResult>(actual.Result);
    }

    [Fact]
    public async void UpdateUser_returns_badrequest_given_not_updated()
    {
        // Arrange
        // User is logged in here...
        var updatedUser = new UserUpdateDTO("Jens", "jens@itu.dk");
        var updateFile = new UploadedFile() {FileContent = new byte[200], FileName = "image.jpg"};
        var updateBody = new UpdateUserBody() {file = updateFile, updatedUser = updatedUser};
        _repository.Setup(m => m.Read(_user.Id)).ReturnsAsync(_user);
        _repository.Setup(m => m.Update(_user.Id, updatedUser)).ReturnsAsync(Status.BadRequest);

        // act
        var actual = await _controller.UpdateUser(updateBody);

        // assert
        Assert.IsType<BadRequestObjectResult>(actual.Result);
    }
}