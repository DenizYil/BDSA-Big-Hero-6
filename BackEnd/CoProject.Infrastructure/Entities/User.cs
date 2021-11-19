using System.ComponentModel.DataAnnotations;

namespace CoProject.Infrastructure.Entities;

public class User
{
    public int Id { get; set; }
    public string Name{ get; set; }
    
    [EmailAddress]
    public string Email{ get; set; }
    
    public bool Supervisor{ get; set; }
    public string ProfileImage{ get; set; }
}