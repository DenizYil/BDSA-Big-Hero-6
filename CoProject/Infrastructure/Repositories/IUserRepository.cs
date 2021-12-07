namespace CoProject.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<UserDetailsDTO?> Read(string id);
    Task<IEnumerable<UserDetailsDTO>> ReadAll();
    Task<IEnumerable<ProjectDetailsDTO>?> ReadAllByUser(string id);
    Task<UserDetailsDTO> Create(UserCreateDTO create);
    Task<Status> Update(string id, UserUpdateDTO update);
    Task<Status> Delete(string id);
}