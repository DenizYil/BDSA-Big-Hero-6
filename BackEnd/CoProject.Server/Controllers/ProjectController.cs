using Microsoft.AspNetCore.Mvc;

namespace CoProject.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ProjectController
{
    [HttpGet(Name = "GetProjects")]
    public IEnumerable<Project> GetProjects()
    {
        return new Project[] {};
    }
    
    [HttpGet(Name = "GetProject")]
    public IEnumerable<Project> GetProject(int id)
    {
        return new Project[] {};
    }
    
    [HttpPost(Name = "CreateProject")]
    public Project CreateProject(Project p)
    {
        return new Project{};
    }
    
    [HttpPut(Name = "ChangeProject")]
    public Project ChangeProject(int id, Project p)
    {
        return new Project{};
    }
    
    [HttpDelete(Name = "DeleteProject")]
    public void ChangeProject(int id)
    {
        return;
    }
}