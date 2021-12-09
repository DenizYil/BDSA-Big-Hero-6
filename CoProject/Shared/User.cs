namespace CoProject.Shared;

public record UserCreateDTO(string Id, string Name, string Email, bool Supervisor);

public record UserUpdateDTO(string Name, string Email);

public record UserDetailsDTO(string Id, string Name, string Email, bool Supervisor, string Image);