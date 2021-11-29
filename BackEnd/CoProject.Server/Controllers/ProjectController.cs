namespace CoProject.Server.Controllers;

[ApiController]
[Route("projects")]
public class ProjectController : ControllerBase
{
    private readonly IProjectRepository _projectRepository;

    public ProjectController(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    [HttpGet("projects")]
    public async Task<IEnumerable<ProjectDetailsDTO>> GetProjects() 
    {
        return await _projectRepository.ReadAll();
    }

    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(ProjectDetailsDTO), 200)]
    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDetailsDTO?>> GetProject(int id)
    {
        var project = await _projectRepository.Read(id);
        
        if (project == null)
        {
            return NotFound();
        }

        return project;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProjectDetailsDTO), 201)]
    public async Task<IActionResult> CreateProject(ProjectCreateDTO project)
    {
        Console.WriteLine(project.Name);
        var projectDetailsDto = await _projectRepository.Create(project);
        return CreatedAtRoute(nameof(GetProject), new {Id = projectDetailsDto.Id}, projectDetailsDto);
    }


    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectUpdateDTO project)
    {
        var response = await _projectRepository.Update(id, project);

        if (response == Status.Updated)
        {
            return NoContent();
        }

        return NotFound();
    }

    [HttpPut("{projectId}/{userId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> AddUserToProject(int projectId, int userId)
    {
        var project = await _projectRepository.Read(projectId);

        if (project == null)
        {
            return NotFound();
        }
        
        var users = project.Users.Select(u => u.Id).ToList();
        users.Add(userId);

        await _projectRepository.Update(projectId, new ProjectUpdateDTO{Users = users});
        return NoContent();
    }
    
    [HttpDelete("{ProjectId}/{UserId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RemoveUserFromProject(int ProjectId, int UserId)
    {
        var project = await _projectRepository.Read(ProjectId);

        if (project == null)
        {
            return NotFound();
        }

        var users = project.Users.Select(u => u.Id).ToList();

        users.Remove(UserId);

        await _projectRepository.Update(ProjectId, new ProjectUpdateDTO(){Users = users});
        return NoContent();
    }


    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var response = await _projectRepository.Delete(id);

        if (response == Status.Deleted)
        {
            return NoContent();
        }

        return NotFound();
    }
}