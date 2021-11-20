using CoProject.Infrastructure.Entities;

namespace CoProject.Infrastructure.DTOs;

public record ProjectCreateDTO 
{
    public string Name { get; init; }
    public string Description { get; init; }
    public int SupervisorId { get; init; }
    public int? Min { get; init; }
    public int? Max { get; init; }
    
    public State State { get; init; }
    public IReadOnlyCollection<string> Tags { get; init; }
}

public record ProjectDTO : ProjectCreateDTO
{
    public int Id { get; init; }
    public DateTime Created { get; init; }
    public IReadOnlyCollection<UserDTO> Users { get; init; }
}

// TODO: Tjek om det her er en god idé?
public record ProjectUpdateDTO
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public int? Min { get; init; }
    public int? Max { get; init; }
    public State? State { get; init; }
    public IReadOnlyCollection<string>? Tags { get; init; }
}