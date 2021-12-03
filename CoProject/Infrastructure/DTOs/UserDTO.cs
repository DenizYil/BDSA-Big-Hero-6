using CoProject.Infrastructure.Entities;

namespace CoProject.Infrastructure.DTOs;

public record UserCreateDTO(
    int Id, string UserName, string NormalizedUserName,
    string Email, string NormalizedEmail, string PhoneNumber,
    string ConcurrencyStamp, string PasswordHash, string SecurityStamp,
    bool Supervisor, bool EmailConfirmed, bool LockoutEnabled, bool PhoneNumberConfirmed, bool TwoFactorEnabled,
    int AccessFailedCount, IReadOnlyCollection<Project> Projects
)
{
    public DateTimeOffset? LockoutEnd { get; set; }
}

public record UserUpdateDTO(string Name, string Email);

public record UserDetailsDTO(int Id, string Name, string UserName, string Email) : UserUpdateDTO(Name, Email);