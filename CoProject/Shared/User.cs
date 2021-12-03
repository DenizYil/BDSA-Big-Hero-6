
namespace CoProject.Shared;

public record UserCreateDTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public IReadOnlyCollection<ProjectDetailsDTO> Projects { get; set; }
    public bool Supervisor { get; set; }
}

public record UserUpdateDTO
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public record UserDetailsDTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}