using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

public class loginBody
{
    public string? username { get; set; }
    public string? password { get; set; }
}

namespace CoProject.Server.Controllers
{
    [ApiController]
    [Route("authentication")]
    public class AuthController
    {

        [HttpPost]
        [Route("login")]
        public String Login([Required][FromBody] loginBody body)
        {
            return "Logged in!";
        }

    }
}

