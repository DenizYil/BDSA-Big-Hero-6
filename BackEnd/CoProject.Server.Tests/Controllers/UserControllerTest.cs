using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace CoProject.Server.Tests.Controllers;

public class UserControllerTest
{
    private static readonly Mock<IUserRepository> repository = new ();
    private static readonly UserController controller = new (repository.Object);

    [Fact]
    public async void GetUsers_returns_all_users()
    {
        //Arrange
        var expected = Array.Empty<UserDetailsDTO>(); 
        repository.Setup(m => m.ReadAll()).ReturnsAsync(expected);

        //Act
        var actual = await controller.GetUsers();
        
        //Assert
        Assert.Equal(expected, actual);
    }
    
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
}