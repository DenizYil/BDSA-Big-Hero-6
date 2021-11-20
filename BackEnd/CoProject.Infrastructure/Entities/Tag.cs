namespace CoProject.Infrastructure.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }

    public IReadOnlyCollection<Project> Projects { get; set; }
}