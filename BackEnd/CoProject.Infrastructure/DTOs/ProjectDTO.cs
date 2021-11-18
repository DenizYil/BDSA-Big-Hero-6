namespace CoProject.Infrastructure.DTOs;

public record ProjectDTO(int Id, string Name, string Description, DateTime Created, int SupervisorId, int? Min, int? Max);

