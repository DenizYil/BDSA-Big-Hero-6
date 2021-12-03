namespace CoProject.Infrastructure.Entities;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime Created { get; set; }
    public int SupervisorId { get; set; }
    public int? Min { get; set; }
    public int? Max { get; set; }
    public IReadOnlyCollection<Tag> Tags { get; set; } = null!;
    public IReadOnlyCollection<User> Users { get; set; } = null!;
    public State State { get; set; }
}