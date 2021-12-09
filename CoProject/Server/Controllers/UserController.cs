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
    private readonly IWebHostEnvironment _env;

    public UserController(IUserRepository userRepository, IWebHostEnvironment env)
    {
        _userRepository = userRepository;
        _env = env;
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
    public async Task<ActionResult<Status>> UpdateUser([FromBody] UpdateUserBody body)
    {
        var idFind = User.FindFirst(e => e.Type == ClaimConstants.ObjectId);

        if (idFind == null)
        {
            return Unauthorized("You are not logged in");
        }

        if(body.file != null)
        {
            var path = $"{_env.WebRootPath}/userimages/{idFind.Value}.jpg";
            var fs = System.IO.File.Create(path);
            fs.Write(body.file.FileContent, 0, body.file.FileContent.Length);
            fs.Close();

            Console.WriteLine("PATH PATH: " + path);
        }


        // WE NEED TO SET THE USER IMAGE TO /userimages/idFind.Value.jpg

        

        var status = await _userRepository.Update(idFind.Value, body.updatedUser);

        if (status == Status.Updated)
        {
            return Ok("You have successfully updated your information");
        }

        return BadRequest("Your information could not be updated");
    }
}