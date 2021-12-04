using CoProject.Infrastructure.Entities;

namespace CoProject.Shared;

public record ProjectCreateDTO(string Name, string Description, int SupervisorId, State State, IReadOnlyCollection<string> Tags)
{
    public int? Min { get; init; }
    public int? Max { get; init; }
}

public record ProjectDetailsDTO(
    int Id, 
    string Name, 
    string Description, 
    int SupervisorId, 
    State State,
    DateTime Created, 
    IReadOnlyCollection<string> Tags, 
    IReadOnlyCollection<UserDetailsDTO> Users
) : ProjectCreateDTO(Name, Description, SupervisorId, State, Tags);

public record ProjectUpdateDTO
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public int? Min { get; init; }
    public int? Max { get; init; }
    public State? State { get; init; }
    public IReadOnlyCollection<string>? Tags { get; init; }
    public IReadOnlyCollection<string>? Users { get; init; }
}