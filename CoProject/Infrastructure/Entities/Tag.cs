namespace CoProject.Infrastructure.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Project> Projects { get; set; } = new List<Project>();
}