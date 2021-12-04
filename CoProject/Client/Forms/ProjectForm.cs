using System.ComponentModel.DataAnnotations;
using CoProject.Infrastructure.Entities;

namespace CoProject.Client.Forms;

public class ProjectForm
{
    [Required]
    [StringLength(100, ErrorMessage = "Name is too long")]
    public string Name { get; set; }

    [Required] public string Description { get; set; }

    public int? Min { get; set; }
    public int? Max { get; set; }

    public string? Tags { get; set; }

    public State State { get; set; }
}