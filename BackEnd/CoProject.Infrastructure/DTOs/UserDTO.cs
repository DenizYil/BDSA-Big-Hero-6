namespace CoProject.Infrastructure.DTOs;

public record UserDTO
{
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}