using System.ComponentModel.DataAnnotations;

namespace CoProject.Client.Forms;

public class ProfileForm
{
    [Required]
    [StringLength(50, ErrorMessage = "Name cannot be more than 50 characters")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100, ErrorMessage = "Email cannot be more than 50 characters")]
    public string Email { get; set; } = string.Empty;

    [Required]
    public bool Supervisor { get; set; }
}