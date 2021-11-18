using Microsoft.AspNetCore.Mvc;

namespace CoProject.Server.Controllers;

// THIS SHOULD BE REPLACED BY REAL PROJECT CLASS
// FROM INFRASTRUCTURE
public class Project
{
    public string? Name { get; init; }
    public int? Id { get; init; }
}

[ApiController]
[Route("projects")]
public class ProjectController
{

    [HttpGet]
    public IEnumerable<Project> GetProjects()
    {
        return new List<Project>();

    }

    [HttpGet]
    [Route("{id}")]
    public Project GetProject(int id)
    {
        return new Project() { Id = id };
    }

    [HttpPost]
    public Project CreateProject(Project p)
    {
        return new Project() { Id = p.Id, Name = p.Name};
    }
    
    [HttpPut]
    [Route("{id}")]
    public Project ChangeProject(int id, Project p)
    {
        return new Project() { Id = id, Name = p.Name };
    }
    
    [HttpDelete]
    [Route("{id}")]
    public Project DeleteProject(int id)
    {
        return new Project() { Id = id };
    }
}