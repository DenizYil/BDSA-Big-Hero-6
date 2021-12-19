using System.ComponentModel.DataAnnotations;

namespace CoProject.Client.Forms;

public class ProfileForm
{
    [Required]
    [StringLength(100, ErrorMessage = "Name is too long")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100, ErrorMessage = "Email is too long")]
    public string Email { get; set; } = string.Empty;

    [Required]
    public bool Supervisor { get; set; }
}