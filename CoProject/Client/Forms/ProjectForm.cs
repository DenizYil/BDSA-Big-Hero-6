using System.ComponentModel.DataAnnotations;
using CoProject.Client.Validators;
using CoProject.Infrastructure.Entities;

namespace CoProject.Client.Forms;

public class ProjectForm
{
    [Required]
    [StringLength(50, ErrorMessage = "Name cannot be more than 50 characters")]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [CustomValidation(typeof(ProjectValidation), nameof(ProjectValidation.ValidateMinMax))]
    public int? Min { get; set; }

    [CustomValidation(typeof(ProjectValidation), nameof(ProjectValidation.ValidateMinMax))]
    public int? Max { get; set; }

    public string? Tags { get; set; }

    public State State { get; set; }
}