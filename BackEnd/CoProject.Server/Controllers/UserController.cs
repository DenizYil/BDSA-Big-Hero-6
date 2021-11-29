using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoProject.Server.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    
    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    [HttpGet]
    public async Task<IEnumerable<UserDetailsDTO>> GetUsers() 
    {
        return await _userRepository.ReadAll();
    }
    
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(UserDetailsDTO), 200)]
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDetailsDTO?>> GetUser(int id)
    {
        Console.WriteLine("Det er her!");
        var user = await _userRepository.Read(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    [ProducesResponseType(201)]
    [HttpPost]
    public async Task<IActionResult> CreateUser(UserCreateDTO newUser)
    {
        var user = await _userRepository.Create(newUser);
        return CreatedAtRoute(nameof(GetUser), new {user.Id}, user);
    }
    
    /*[HttpGet]
    [Route("{id}")]
    public String? GetUser(int id)
    {
        Console.WriteLine("ehh what?");
        if (User.Identity != null)
        {
            return User.Identity.Name?.ToString();
        }else
        {
            return "no user";
        }
    }*/

    [ProducesResponseType(404)]
    [ProducesResponseType(204)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
       var response = await _userRepository.Delete(id);

       if (response == Status.Deleted)
       {
           return NoContent();
       }
       
       return NotFound();
    }
    
    [ProducesResponseType(404)]
    [ProducesResponseType(204)]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDTO updatedUser)
    {
        var response = await _userRepository.Update(id, updatedUser);

        if (response == Status.Updated)
        {
            return NoContent();
        }

        return NotFound();
    }
}