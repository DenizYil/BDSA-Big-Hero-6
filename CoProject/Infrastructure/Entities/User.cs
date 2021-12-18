namespace CoProject.Infrastructure.Entities;

public class User
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool Supervisor { get; set; }
    public string Image { get; set; } = "/images/noimage.jpeg";

    public ICollection<Project> Projects { get; init; } = new List<Project>();
}