using CoProject.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

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
    [HttpGet("{id}", Name = nameof(GetProject))]
    public async Task<ActionResult<ProjectDetailsDTO>> GetProject(int id)
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
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> CreateProject(ProjectCreateDTO project)
    {
        var id = User.FindFirst(e => e.Type == ClaimConstants.ObjectId);

        if (id == null)
        {
            return Unauthorized();
        }
        
        var supervisor = await _userRepository.Read(id.Value);

        if(supervisor != null && supervisor.Supervisor)
        {
            project.SupervisorId = supervisor.Id;

            var created = await _projectRepository.Create(project);
            return CreatedAtRoute(nameof(GetProject), new { created.Id }, created);
        }

        return Forbid();
    }

    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectUpdateDTO project)
    {
        var userId = User.FindFirst(e => e.Type == ClaimConstants.ObjectId);

        if (userId == null)
        {
            return Unauthorized();
        }

        var user = await _userRepository.Read(userId.Value);
        var _project = await _projectRepository.Read(id);

        if(user == null)
        {
            return NotFound();
        }

        if(_project == null)
        {
            return NotFound();
        }

        if(user.Supervisor && _project.Supervisor.Id.Equals(user.Id))
        {
            // The logged in user is supervisor and is creator of the project trying to be deleted
            var response = await _projectRepository.Update(id, project);

            if (response == Status.Updated)
            {
                return NoContent();
            }

            return NotFound();
        }
        return NotFound();

        
    }

    [HttpPut("{projectId}/join")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> AddUserToProject(int projectId)
    {
        var project = await _projectRepository.Read(projectId);

        if (project == null)
        {
            return NotFound();
        }

        var id = User.FindFirst(e => e.Type == ClaimConstants.ObjectId);

        if (id == null)
        {
            return Unauthorized();
        }

        var users = project.Users.Select(u => u.Id).ToList();
        
        if (users.Contains(id.Value))
        {
            return Conflict();
        }
        
        users.Add(id.Value);

        await _projectRepository.Update(projectId, new ProjectUpdateDTO {Users = users});
        return NoContent();
    }

    [HttpDelete("{projectId}/leave")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RemoveUserFromProject(int projectId)
    {
        var project = await _projectRepository.Read(projectId);

        if (project == null)
        {
            return NotFound();
        }

        var id = User.FindFirst(e => e.Type == ClaimConstants.ObjectId);

        if (id == null)
        {
            return Unauthorized();
        }

        var users = project.Users.Select(u => u.Id).ToList();

        if (users.Contains(id.Value))
        {
            if (users.Remove(id.Value))
            {
                var updatedStatus = await _projectRepository.Update(projectId, new ProjectUpdateDTO() { Users = users });
                if(updatedStatus == Status.Updated)
                {
                    return NoContent();
                }else
                {
                    return NotFound();
                }
                
            }else
            {
                return NotFound();
            }
            
        }else
        {
            // The user is not in the project
            return NotFound();
        }



        
    }


    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var userId = User.FindFirst(e => e.Type == ClaimConstants.ObjectId);

        if (userId == null)
        {
            return Unauthorized();
        }

        var project = await _projectRepository.Read(id);
        var user = await _userRepository.Read(userId.Value);

        // Only delete if the user actually owns this project
        if(user != null && user.Supervisor == true && project != null && project.Supervisor.Id.Equals(user.Id))
        {
            var response = await _projectRepository.Delete(id);

            if (response == Status.Deleted)
            {
                return NoContent();
            }

            return NotFound();
        }

        return Unauthorized();

        
    }
}