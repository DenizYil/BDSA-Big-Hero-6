using CoProject.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

namespace CoProject.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    
    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(UserDetailsDTO), 200)]
    [HttpGet]
    public async Task<ActionResult<UserDetailsDTO?>> GetUser()
    {
        var idFind = User.FindFirst(e => e.Type == ClaimConstants.ObjectId);

        if (idFind == null)
        {
            return NotFound();
        }
        
        var user = await _userRepository.Read(idFind.Value);

        if(user == null)
        {
            return NotFound();
        }
        
        return user;
    }
    
    [HttpGet("projects")]
    public async Task<IEnumerable<ProjectDetailsDTO>> GetProjectsByUser()
    {
        
        var idFind = User.FindFirst(e => e.Type == ClaimConstants.ObjectId);

        if (idFind != null)
        {
            return (await _userRepository.ReadAllByUser(idFind.Value)).Where(project => project.State != State.Deleted);
        }
        
        return new List<ProjectDetailsDTO>();
    }

    [ProducesResponseType(201)]
    [HttpPost("signup")]
    public async Task<ActionResult<UserDetailsDTO?>> SignupUser()
    {
        var nameFind = User.FindFirst("name");
        var idFind = User.FindFirst(e => e.Type == ClaimConstants.ObjectId);
        var emailFind = User.FindFirst("emails");

        if (nameFind == null || idFind == null || emailFind == null)
        {
            return NotFound();
        }
        
        var name = nameFind.Value.Trim();
        var userId = idFind.Value.Trim();
        var email = emailFind.Value.Trim();

        var user = await _userRepository.Read(userId);

        if (user == null)
        {
            await _userRepository.Create(new UserCreateDTO(userId, name, email, false));
            return CreatedAtAction(nameof(GetUser), user);
        }
            
        return NoContent();

    }

    [ProducesResponseType(404)]
    [ProducesResponseType(204)]
    [HttpPut]
    public async Task<ActionResult<Status>> UpdateUser([FromBody] UserUpdateDTO updatedUser)
    {
        var idFind = User.FindFirst(e => e.Type == ClaimConstants.ObjectId);

        if (idFind == null)
        {
            return NotFound();
        }
        
        await _userRepository.Update(idFind.Value, updatedUser);
        return NoContent();
    }
}