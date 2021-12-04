namespace CoProject.Infrastructure.Entities;

#pragma warning disable CS8618

public class User
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public bool Supervisor{ get; set; }

    public IReadOnlyCollection<Project> Projects { get; set; } = new List<Project>();
}