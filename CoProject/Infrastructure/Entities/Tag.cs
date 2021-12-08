using System.Collections;

namespace CoProject.Infrastructure.Entities;

#pragma warning disable CS8618

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Project> Projects { get; set; } = new List<Project>();
}