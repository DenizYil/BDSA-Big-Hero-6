using CoProject.Infrastructure.Entities;

namespace CoProject.Infrastructure.DTOs;

public record UserCreateDTO(
    string Id, string Name, string Email, IReadOnlyCollection<Project> Projects, bool Supervisor
);

public record UserUpdateDTO(string Name, string Email);

public record UserDetailsDTO(string Id, string Name, string UserName, string Email) : UserUpdateDTO(Name, Email);