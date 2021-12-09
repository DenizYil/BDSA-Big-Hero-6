using System.Collections.Generic;
using System.Linq;
using CoProject.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace CoProject.Server.Tests.Controllers;

public class ProjectControllerTests
{
    private readonly Mock<IProjectRepository> _projectRepository;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly ProjectController _controller;
    private readonly ProjectDetailsDTO _project;

    public ProjectControllerTests()
    {
        _projectRepository = new();
        _userRepository = new ();
        _controller = new(_projectRepository.Object, _userRepository.Object);
        _project = new(1, "Project Name", "Project Description", new UserDetailsDTO("1", "Supervisor", "supervisor@outlook.dk", true, "/images/noimage.jpeg"), State.Open, DateTime.Now, new List<string>(), new List<UserDetailsDTO>());
    }

    [Fact]
    public async void GetProjects_returns_all_projects()
    {
        // Arrange
        _projectRepository.Setup(m => m.ReadAll()).ReturnsAsync(new List<ProjectDetailsDTO>());

        // Act
        var actual = await _controller.GetProjects();

        // Assert
        Assert.Equal(new List<ProjectDetailsDTO>(), actual);
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
}