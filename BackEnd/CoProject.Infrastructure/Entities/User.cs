using Microsoft.AspNetCore.Identity;

namespace CoProject.Infrastructure.Entities;

public class User : IdentityUser<int>
{
    public bool Supervisor{ get; set; }
    
    public IReadOnlyCollection<Project> Projects { get; set; }
}