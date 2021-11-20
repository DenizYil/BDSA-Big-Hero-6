using Microsoft.AspNetCore.Identity;

namespace CoProject.Infrastructure.Entities;

#pragma warning disable CS8618

public class User : IdentityUser<int>
{
    public bool Supervisor{ get; set; }
    
    public IReadOnlyCollection<Project> Projects { get; set; }
}