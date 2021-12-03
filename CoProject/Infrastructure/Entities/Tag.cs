namespace CoProject.Infrastructure.Entities;

#pragma warning disable CS8618

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }

    public IReadOnlyCollection<Project> Projects { get; set; }
}