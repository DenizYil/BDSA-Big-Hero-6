using System.ComponentModel.DataAnnotations;
using CoProject.Client.Forms;

namespace CoProject.Client.Validators;

public class ProjectValidation
{
    public static ValidationResult? ValidateMinMax(int? _, ValidationContext ctx)
    {
        var project = (ProjectForm) ctx.ObjectInstance;

        if (project.Min > project.Max)
        {
            return new ValidationResult("Min cannot be greater than Max.", new[] {nameof(project.Min), nameof(project.Max)});
        }

        return ValidationResult.Success;
    }
}