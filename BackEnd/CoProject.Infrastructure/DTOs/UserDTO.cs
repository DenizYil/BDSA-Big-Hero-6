namespace CoProject.Infrastructure.DTOs;

#pragma warning disable CS8618

public record UserDTO
{
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}