namespace CoProject.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/projects")]
public class ProjectController : ControllerBase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;

    public ProjectController(IProjectRepository projectRepository, IUserRepository userRepository)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<IEnumerable<ProjectDetailsDTO>> GetProjects()
    {
        return (await _projectRepository.ReadAll()).Where(project => project.State == State.Open);
    }

    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(ProjectDetailsDTO), 200)]
    [HttpGet("{id:int}", Name = nameof(GetProject))]
    public async Task<ActionResult<ProjectDetailsDTO>> GetProject(int id)
    {
        var project = await _projectRepository.Read(id);

        if (project == null)
        {
            return NotFound("Project could not be found");
        }

        return project;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProjectDetailsDTO), 201)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> CreateProject(ProjectCreateDTO project)
    {
        var id = User.FindFirst(e => e.Type == ClaimConstants.ObjectId);

        if (id == null)
        {
            return Unauthorized("You are not logged in");
        }

        var supervisor = await _userRepository.Read(id.Value);

        if (supervisor == null)
        {
            return Unauthorized("You are not logged in");
        }

        if (!supervisor.Supervisor)
        {
            return Forbid("You are not a supervisor");
        }

        project.SupervisorId = supervisor.Id;

        var created = await _projectRepository.Create(project);
        return CreatedAtRoute(nameof(GetProject), new {created.Id}, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectUpdateDTO update)
    {
        var userId = User.FindFirst(e => e.Type == ClaimConstants.ObjectId);

        if (userId == null)
        {
            return Unauthorized("You are not logged in");
        }

        var user = await _userRepository.Read(userId.Value);

        if (user == null)
        {
            return Unauthorized("You are not logged in");
        }

        var project = await _projectRepository.Read(id);

        if (project == null)
        {
            return NotFound("Project was not found");
        }

        if (!user.Supervisor || project.Supervisor.Id != user.Id)
        {
            return Forbid("You are not this project's supervisor");
        }

        // The logged in user is supervisor and is creator of the project trying to be deleted
        var response = await _projectRepository.Update(id, update);

        if (response == Status.Updated)
        {
            return Ok("Project has been successfully updated!");
        }

        // Should never hit this
        return BadRequest("Project could not be updated");
    }

    [HttpPut("{projectId:int}/join")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<ActionResult> AddUserToProject(int projectId)
    {
        var project = await _projectRepository.Read(projectId);

        if (project == null)
        {
            return NotFound("Project was not found");
        }

        if (project.State != State.Open)
        {
            return Forbid("The project is not open");
        }

        if (project.Users.Count >= project.Max)
        {
            return Forbid("The project is full");
        }

        var id = User.FindFirst(e => e.Type == ClaimConstants.ObjectId);

        if (id == null)
        {
            return Unauthorized("You are not logged in");
        }

        var users = project.Users.Select(u => u.Id).ToList();

        if (users.Contains(id.Value))
        {
            return Conflict("You have already joined this project");
        }

        users.Add(id.Value);

        var updatedStatus = await _projectRepository.Update(projectId, new() {Users = users});

        if (updatedStatus == Status.Updated)
        {
            return Ok("You have joined the project");
        }

        return BadRequest("You could not join the project");
    }

    [HttpDelete("{projectId:int}/leave")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> RemoveUserFromProject(int projectId)
    {
        var project = await _projectRepository.Read(projectId);

        if (project == null || project.State == State.Deleted)
        {
            return NotFound("Project was not found");
        }

        var id = User.FindFirst(e => e.Type == ClaimConstants.ObjectId);

        if (id == null)
        {
            return Unauthorized("You are not logged in");
        }

        var users = project.Users.Select(u => u.Id).ToList();

        if (!users.Remove(id.Value))
        {
            return Conflict("You are not apart of the project");
        }

        var updatedStatus = await _projectRepository.Update(projectId, new() {Users = users});

        if (updatedStatus == Status.Updated)
        {
            return Ok("You have left the project");
        }

        return BadRequest("You could not leave the project");
    }

    [HttpDelete("{projectId:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteProject(int projectId)
    {
        var userId = User.FindFirst(e => e.Type == ClaimConstants.ObjectId);

        if (userId == null)
        {
            return Unauthorized("You are not logged in");
        }

        var user = await _userRepository.Read(userId.Value);

        if (user == null)
        {
            return Unauthorized("You are not logged in");
        }

        if (!user.Supervisor)
        {
            return Forbid("You are not a supervisor");
        }

        var project = await _projectRepository.Read(projectId);

        if (project == null)
        {
            return NotFound("Project was not found");
        }

        if (project.Supervisor.Id != user.Id)
        {
            return Forbid("You are not this project's supervisor");
        }

        var response = await _projectRepository.Delete(projectId);

        if (response == Status.Deleted)
        {
            return Ok("Project was deleted");
        }

        return BadRequest("Project could not be deleted");
    }
}