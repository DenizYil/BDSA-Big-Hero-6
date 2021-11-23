using CoProject.Infrastructure.DTOs;

namespace CoProject.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<UserDetailsDTO?> Read(int id);
    Task<IEnumerable<UserDetailsDTO>> ReadAll();
    Task<UserDetailsDTO> Create(UserCreateDTO create);
    Task<Status> Update(int id, UserUpdateDTO update);
    Task<Status> Delete(int id);
}