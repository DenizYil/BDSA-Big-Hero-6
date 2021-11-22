namespace CoProject.Infrastructure.DTOs;

#pragma warning disable CS8618

public record UserCreateDTO
{
    
}

public record UserUpdateDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public record UserDetailsDTO
{
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}