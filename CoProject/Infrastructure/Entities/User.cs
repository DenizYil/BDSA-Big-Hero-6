using System.Collections;

namespace CoProject.Infrastructure.Entities;

#pragma warning disable CS8618

public class User
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public bool Supervisor { get; set; }
    public string Image { get; set; } = "/images/noimage.jpeg";

    public ICollection<Project> Projects { get; set; } = new List<Project>();
}