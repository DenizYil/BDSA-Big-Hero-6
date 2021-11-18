namespace CoProject.Infrastructure.Entities;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime Created { get; set; }
    public int SupervisorId { get; set; }
    public int? Min { get; set; }
    public int? Max { get; set; }
    public int StateId { get; set; }
}