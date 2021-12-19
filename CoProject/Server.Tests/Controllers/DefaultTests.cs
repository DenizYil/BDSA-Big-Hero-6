using System.Security.Claims;
using System.Security.Principal;
using Microsoft.Identity.Web;

namespace CoProject.Server.Tests.Controllers;

public class DefaultTests
{
    protected readonly UserDetailsDTO User;
    protected readonly ControllerContext ControllerContext;

    protected DefaultTests()
    {
        User = new("123", "Test User", "user@outlook.com", false, "/images/noimage.jpeg");

        var identity = new GenericIdentity(User.Name, "");
        identity.AddClaim(new(ClaimConstants.Name, User.Name));
        identity.AddClaim(new("emails", User.Email));
        identity.AddClaim(new(ClaimConstants.ObjectId, User.Id));
        
        var principal = new GenericPrincipal(identity, new string[] { });
        var loggedInUser = new ClaimsPrincipal(principal);
        
        ControllerContext = new() {HttpContext = new DefaultHttpContext {User = loggedInUser}};
    }
}