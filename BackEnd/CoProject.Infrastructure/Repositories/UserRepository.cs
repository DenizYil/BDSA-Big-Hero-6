using CoProject.Infrastructure.DTOs;

namespace CoProject.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private ICoProjectContext _context;

    public UserRepository(ICoProjectContext context)
    {
        _context = context;
    }
    
    public async Task<UserDetailsDTO?> Read(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<UserDetailsDTO>> ReadAll()
    {
        throw new NotImplementedException();
    }

    public async Task<UserDetailsDTO> Create(UserCreateDTO create)
    {
        throw new NotImplementedException();
    }

    public async Task<Status> Update(UserUpdateDTO update)
    {
        throw new NotImplementedException();
    }

    public async Task<Status> Delete(int id)
    {
        throw new NotImplementedException();
    }
}