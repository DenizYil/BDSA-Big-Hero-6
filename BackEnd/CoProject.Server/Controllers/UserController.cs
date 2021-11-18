using Microsoft.AspNetCore.Mvc;

namespace CoProject.Server.Controllers;

// THIS SHOULD BE REPLACED BY REAL PROJECT CLASS
// FROM INFRASTRUCTURE
public class User
{
    public string? Name { get; init; }
    public int? Id { get; init; }
}

[ApiController]
[Route("users")]
public class UserController
{

    [HttpGet]
    [Route("{id}")]
    public Project GetUser(int id)
    {
        return new Project() { Id = id };
    }
}