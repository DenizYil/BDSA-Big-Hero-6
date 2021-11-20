namespace CoProject.Infrastructure.Entities;

#pragma warning disable CS8618

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Created { get; set; }
    public int SupervisorId { get; set; }
    public int? Min { get; set; }
    public int? Max { get; set; }
    public IReadOnlyCollection<Tag> Tags { get; set; }
    public IReadOnlyCollection<User> Users { get; set; }
    public State State { get; set; }
}