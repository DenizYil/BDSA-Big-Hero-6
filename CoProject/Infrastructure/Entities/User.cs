using Microsoft.AspNetCore.Identity;

namespace CoProject.Infrastructure.Entities;

#pragma warning disable CS8618

public class User : IdentityUser<string>
{
    public bool Supervisor{ get; set; }
    
    public IReadOnlyCollection<Project> Projects { get; set; }
}