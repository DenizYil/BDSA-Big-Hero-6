using CoProject.Infrastructure.Entities;

namespace CoProject.Infrastructure.DTOs;

#pragma warning disable CS8618

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

public record ProjectDetailsDTO : ProjectCreateDTO
{
    public int Id { get; init; }
    public DateTime Created { get; init; }
    public IReadOnlyCollection<UserDTO> Users { get; init; }
}

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