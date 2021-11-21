namespace CoProject.Server.Controllers;

// THIS SHOULD BE REPLACED BY REAL PROJECT CLASS
// FROM INFRASTRUCTURE

[ApiController]
[Route("projects")]
public class ProjectController : ControllerBase
{

    private readonly IProjectRepository _projectRepository;

    public ProjectController(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }
    
    [HttpGet]
    public async Task<IEnumerable<ProjectDTO>> GetProjects()
        => await _projectRepository.ReadAll();

    [ProducesResponseType(404)]
    [HttpGet("{id}")]
    public async Task<ProjectDTO?> GetProject(int id)
        => await _projectRepository.Read(id);

    [HttpPost]
    public async Task<IActionResult> CreateProject(ProjectCreateDTO project)
    {
        var (_, id) = await _projectRepository.Create(project);

        return CreatedAtRoute(nameof(GetProject), new {Id = id}, null);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectUpdateDTO project)
    {
        var response = await _projectRepository.Update(project);
        

        if (response == Status.Updated)
        {
            return NoContent();
        }

        return NotFound();
    }

    
    [HttpDelete]
    [Route("{id}")]
    public Project DeleteProject(int id)
    {
        return new Project() { Id = id };
    }
}