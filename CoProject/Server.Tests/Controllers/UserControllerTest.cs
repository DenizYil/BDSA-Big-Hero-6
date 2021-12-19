namespace CoProject.Server.Tests.Controllers;

public class UserControllerTest : DefaultTests
{
    private readonly Mock<IWebHostEnvironment> _env;
    private readonly Mock<FileStream> _file;
    private readonly Mock<FileBase> _fileWriter;
    private readonly Mock<IUserRepository> _repository;
    private UserController _controller;

    public UserControllerTest()
    {
        // SETUP
        _env = new();
        _fileWriter = new();
        _file = new();
        _env.Setup(m => m.WebRootPath).Returns("../../../");
        _repository = new();
        
        _controller = new(_repository.Object, _env.Object)
        {
            ControllerContext = ControllerContext
        };
    }

    [Fact]
    public async void GetUser_returns_no_token_found()
    {
        // Arrange
        // There is no token
        _controller = new(_repository.Object, _env.Object)
        {
            ControllerContext = new() {HttpContext = new DefaultHttpContext {User = new()}}
        };

        // Act
        var actual = await _controller.GetUser();

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(actual.Result);
    }

    [Fact]
    public async void GetUser_returns_not_found_in_database()
    {
        // Arrange
        // User is logged in but not found in database
        _repository
            .Setup(m => m.Read(User.Id))
            .ReturnsAsync(default(UserDetailsDTO));

        // Act
        var actual = await _controller.GetUser();

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(actual.Result);
    }

    [Fact]
    public async void GetUser_returns_logged_in_user()
    {
        // Arrange
        _repository
            .Setup(m => m.Read(User.Id))
            .ReturnsAsync(User);

        // Act
        var actual = await _controller.GetUser();

        // Assert
        Assert.Equal(User, actual.Value);
    }

    [Fact]
    public async void getProjectsByUser_returns_found_projects()
    {
        // Arrange
        var projects = new List<ProjectDetailsDTO>
        {
            new(1, "Project Name", "Project Description", User, State.Open, DateTime.Now, new List<string>(), new List<UserDetailsDTO>())
        };
        _repository
            .Setup(m => m.ReadAllByUser(User.Id))
            .ReturnsAsync(projects);

        // Act
        var actual = await _controller.GetProjectsByUser();

        // Assert
        Assert.Equal(projects, actual);
    }

    [Fact]
    public async void getProjectsByUser_returns_empty()
    {
        // Arrange
        _controller = new(_repository.Object, _env.Object)
        {
            ControllerContext = new() {HttpContext = new DefaultHttpContext {User = new()}}
        };

        // Act
        var actual = await _controller.GetProjectsByUser();

        Assert.Equal(new List<ProjectDetailsDTO>(), actual);
    }

    [Fact]
    public async void SignupUser_returns_unauthorized_given_no_token()
    {
        // Arrange
        _controller = new(_repository.Object, _env.Object)
        {
            ControllerContext = new() {HttpContext = new DefaultHttpContext {User = new()}}
        };

        // Act
        var actual = await _controller.SignupUser();

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(actual.Result);
    }

    [Fact]
    public async void SignupUser_returns_created_user_given_not_found_in_db()
    {
        // Arrange
        var userCreate = new UserCreateDTO(User.Id, User.Name, User.Email, User.Supervisor);
        
        _repository
            .Setup(m => m.Read(User.Id))
            .ReturnsAsync(default(UserDetailsDTO));
       
        _repository
            .Setup(m => m.Create(userCreate))
            .ReturnsAsync(User);

        // Act
        var response = await _controller.SignupUser();

        // Assert
        var okObjectResult = response.Result as OkObjectResult;
        Assert.NotNull(okObjectResult);
        var actual = okObjectResult!.Value as UserDetailsDTO;
        Assert.Equal(User, actual);
    }

    [Fact]
    public async void SignupUser_returns_already_found_user()
    {
        // Arrange
        _repository
            .Setup(m => m.Read(User.Id))
            .ReturnsAsync(User);

        // Act
        var response = await _controller.SignupUser();

        // Assert
        var okObjectResult = response.Result as OkObjectResult;
        Assert.NotNull(okObjectResult);
        var actual = okObjectResult!.Value as UserDetailsDTO;
        Assert.Equal(User, actual);
    }

    [Fact]
    public async void UpdateUser_returns_unauthorized_given_no_token()
    {
        // Arrange
        _controller = new(_repository.Object, _env.Object)
        {
            ControllerContext = new() {HttpContext = new DefaultHttpContext {User = new()}}
        };
        
        // Act
        var actual = await _controller.UpdateUser(new(User.Name, User.Email));

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(actual.Result);
    }

    [Fact]
    public async void UpdateUser_returns_badrequest_given_no_jpg_or_png()
    {
        // Arrange
        var update = new UserUpdateDTO(User.Name, User.Email)
        {
            Image = new("image.pdf", new byte[200])
        };
        
        // Act
        var actual = await _controller.UpdateUser(update);

        // Assert
        Assert.IsType<BadRequestObjectResult>(actual.Result);
    }

    [Fact]
    public async void UpdateUser_returns_unauthorized_given_user_not_found_in_db()
    {
        // Arrange
        var update = new UserUpdateDTO(User.Name, User.Email)
        {
            Image = new("image.jpg", new byte[200])
        };
        
        // Act
        var actual = await _controller.UpdateUser(update);
        
        // Assert
        Assert.IsType<UnauthorizedObjectResult>(actual.Result);
    }

    [Fact]
    public async void UpdateUser_returns_ok_given_profile_is_updated()
    {
        // Arrange
        var update = new UserUpdateDTO(User.Name, User.Email)
        {
            Image = new("image.jpg", new byte[200])
        };
        
        // User is logged in here...
        _repository
            .Setup(m => m.Read(User.Id))
            .ReturnsAsync(User);
        
        _fileWriter
            .Setup(m => m.Create($"/userimages/{User.Id}_{Guid.NewGuid()}.jpg"))
            .Returns(() => new FileStream("", FileMode.Create));
        
        _file
            .Setup(m => m.Write(update.Image.Content, 0, update.Image.Content.Length))
            .Verifiable();
        
        _repository
            .Setup(m => m.Update(User.Id, update))
            .ReturnsAsync(Status.Updated);

        // Act
        var actual = await _controller.UpdateUser(update);
        
        // Assert
        Assert.IsType<OkObjectResult>(actual.Result);
    }

    [Fact]
    public async void UpdateUser_returns_badrequest_given_not_updated()
    {
        // Arrange
        // User is logged in here...
        var update = new UserUpdateDTO("Jens", "jens@itu.dk")
        {
            Image = new("image.jpg", new byte[200])
        };

        _repository
            .Setup(m => m.Read(User.Id))
            .ReturnsAsync(User);
        
        _repository
            .Setup(m => m.Update(User.Id, update))
            .ReturnsAsync(Status.BadRequest);

        // Act
        var actual = await _controller.UpdateUser(update);

        // Assert
        Assert.IsType<BadRequestObjectResult>(actual.Result);
    }
}