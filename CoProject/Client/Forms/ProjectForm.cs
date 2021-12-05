using System.ComponentModel.DataAnnotations;
using CoProject.Client.Validators;
using CoProject.Infrastructure.Entities;

namespace CoProject.Client.Forms;

public class ProjectForm
{
    [Required]
    [StringLength(100, ErrorMessage = "Name is too long")]
    public string Name { get; set; }

    [Required] 
    public string Description { get; set; }

    [CustomValidation(typeof(ProjectValidation), nameof(ProjectValidation.ValidateMinMax))]
    public int? Min { get; set; }
    
    [CustomValidation(typeof(ProjectValidation), nameof(ProjectValidation.ValidateMinMax))]
    public int? Max { get; set; }

    public string? Tags { get; set; }

    public State State { get; set; }
}