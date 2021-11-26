namespace CoProject.FrontEnd;

#pragma warning disable CS8618

public record ProjectCreateDTO 
{
    public string Name { get; init; }
    public string Description { get; init; }
    public int SupervisorId { get; init; }
    public int? Min { get; init; }
    public int? Max { get; init; }
    
    public string State { get; init; }
    public IReadOnlyCollection<string> Tags { get; init; }
}

public record ProjectDetailsDTO : ProjectCreateDTO
{
    public int Id { get; init; }
    public DateTime Created { get; init; }
    public IReadOnlyCollection<UserDetailsDTO> Users { get; init; }
}

public record ProjectUpdateDTO
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public int? Min { get; init; }
    public int? Max { get; init; }
    public string? State { get; init; }
    public IReadOnlyCollection<string>? Tags { get; init; }
    public IReadOnlyCollection<int>? Users { get; init; }
}