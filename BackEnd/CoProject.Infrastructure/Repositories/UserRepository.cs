using CoProject.Infrastructure.DTOs;
using Microsoft.EntityFrameworkCore;

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
        return await _context.Users
            .Where(u => u.Id == id)
            .Select(u => new UserDetailsDTO
            {
                UserName = u.NormalizedUserName,
                Email = u.Email,
                Name = u.UserName
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserDetailsDTO>> ReadAll()
    {
        return await _context.Users
            .Select(u => new UserDetailsDTO
            {
                UserName = u.NormalizedUserName,
                Email = u.Email,
                Name = u.UserName
            })
            .ToListAsync();
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
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return Status.NotFound;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Status.Deleted;
    }
}