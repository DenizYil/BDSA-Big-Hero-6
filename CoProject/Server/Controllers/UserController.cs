using CoProject.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using System.IO;
using System;

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

            if(!body.file.FileName.EndsWith(".jpg") && !body.file.FileName.EndsWith(".png"))
            {
                return BadRequest("You must only upload .jpg or .png images");
            }

            var currentUser = await _userRepository.Read(idFind.Value);

            if(currentUser == null)
            {
                return Unauthorized("You are not logged in");
            }

            var previousImagePath = $"{_env.WebRootPath}{currentUser.Image}";
            if ( currentUser.Image != null && System.IO.File.Exists(previousImagePath))
            {
                // delete it
                System.IO.File.Delete(previousImagePath);
            }

            var userImagePath = $"/userimages/{idFind.Value}_{Guid.NewGuid()}.jpg";
            var path = $"{_env.WebRootPath}{userImagePath}";
            var fs = System.IO.File.Create(path);
            fs.Write(body.file.FileContent, 0, body.file.FileContent.Length);
            fs.Close();

            Console.WriteLine("PATH PATH: " + path);
            body.updatedUser.Image = userImagePath;
        }

        

        var status = await _userRepository.Update(idFind.Value, body.updatedUser);

        if (status == Status.Updated)
        {
            return Ok("You have successfully updated your information");
        }

        return BadRequest("Your information could not be updated");
    }
}