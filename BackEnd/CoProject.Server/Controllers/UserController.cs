using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoProject.Server.Controllers;

[Authorize]
[ApiController]
[Route("users")]
public class UserController
{

    public System.Security.Principal.IPrincipal User { get; }

    [HttpGet]
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
    }
}