using System.ComponentModel.DataAnnotations.Schema;

namespace CoProject.Infrastructure.Entities;

public class ProjectTag
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int TagId { get; set; }
}