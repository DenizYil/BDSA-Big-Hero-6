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

    [HttpGet]
    [ProducesResponseType(typeof(UserDetailsDTO), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<UserDetailsDTO?>> GetUser()
    {
        var idFind = User.FindFirst(e => e.Type == ClaimConstants.ObjectId);

        if (idFind == null)
        {
            return Unauthorized("You are not logged in");
        }

        var user = await _userRepository.Read(idFind.Value);

        if (user == null)
        {
            return Unauthorized("You are not logged in");
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

    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<UserDetailsDTO?>> SignupUser()
    {
        var nameFind = User.FindFirst("name");
        var idFind = User.FindFirst(e => e.Type == ClaimConstants.ObjectId);
        var emailFind = User.FindFirst("emails");

        if (nameFind == null || idFind == null || emailFind == null)
        {
            return BadRequest("Your credentials were invalid");
        }

        var name = nameFind.Value.Trim();
        var userId = idFind.Value.Trim();
        var email = emailFind.Value.Trim();

        var user = await _userRepository.Read(userId);

        if (user == null)
        {
            user = await _userRepository.Create(new UserCreateDTO(userId, name, email, false));
        }

        return Ok(user);
    }

    [HttpPut]
    [ProducesResponseType(404)]
    [ProducesResponseType(204)]
    public async Task<ActionResult<Status>> UpdateUser([FromBody] UserUpdateDTO updatedUser)
    {
        var idFind = User.FindFirst(e => e.Type == ClaimConstants.ObjectId);

        if (idFind == null)
        {
            return Unauthorized("You are not logged in");
        }

        var status = await _userRepository.Update(idFind.Value, updatedUser);

        if (status == Status.Updated)
        {
            return Ok("You have successfully updated your information");
        }

        return BadRequest("Your information could not be updated");
    }
}