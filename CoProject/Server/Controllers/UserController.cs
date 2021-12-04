using CoProject.Shared;

namespace CoProject.Server.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

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
        var idFind = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier");

        if(idFind != null)
        {
            var user = await _userRepository.Read(idFind.Value);

            if(user == null)
            {
                return NotFound();
            }
            return user;
        }
        return NotFound();
        
    }
    
    [HttpGet("projects")]
    public async Task<IEnumerable<ProjectDetailsDTO>> GetProjectsByUser()
    {
        var idFind = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier");

        if (idFind != null)
        {
            return await _userRepository.ReadAllByUser(idFind.Value);
        }
        return new List<ProjectDetailsDTO>();
    }

    [ProducesResponseType(201)]
    [HttpPost]
    public async Task<IActionResult> CreateUser(UserCreateDTO newUser)
    {
        var user = await _userRepository.Create(newUser);
        return CreatedAtRoute(nameof(GetUser), new {user.Id}, user);
    }

    [ProducesResponseType(201)]
    [HttpPost("signup")]
    public async Task<IActionResult> SignupUser()
    {

        var nameFind = User.FindFirst("name");
        var idFind = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier");
        var emailFind = User.FindFirst("emails");

        if (nameFind != null && idFind != null && emailFind != null)
        {
            var name = nameFind.Value.Trim();
            var userId = idFind.Value.Trim();
            var email = emailFind.Value.Trim();

            Console.WriteLine("NAME: " + name);
            Console.WriteLine("USER ID: " + userId);
            Console.WriteLine("EMAIL: " + email);

            var isUserAlready = await _userRepository.Read(userId);

            if(isUserAlready == null)
            {
                var newUser = new UserCreateDTO(userId, name, email, false);
                var user = await _userRepository.Create(newUser);
                return CreatedAtRoute(nameof(GetUser), new { user.Id }, user);
            }
            return NoContent();
        }else
        {
            return NotFound();
        }
    }

    [ProducesResponseType(404)]
    [ProducesResponseType(204)]
    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDTO updatedUser)
    {

        var idFind = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier");

        if (idFind != null)
        {
            var response = await _userRepository.Update(idFind.Value, updatedUser);

            if (response == Status.Updated)
            {
                return NoContent();
            }
            return NoContent();
        }
        return NotFound();
    }
}