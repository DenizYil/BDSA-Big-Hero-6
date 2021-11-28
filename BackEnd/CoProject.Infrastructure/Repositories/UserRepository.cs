using CoProject.Infrastructure.DTOs;
using CoProject.Infrastructure.Entities;
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
            .Select(u => new UserDetailsDTO(u.Id, u.UserName, u.NormalizedUserName, u.Email))
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserDetailsDTO>> ReadAll()
    {
        return await _context.Users
            .Select(u => new UserDetailsDTO(u.Id, u.UserName, u.NormalizedUserName, u.Email))
            .ToListAsync();
    }

    public async Task<UserDetailsDTO> Create(UserCreateDTO create)
    {
        var user = new User
        {
            Id = create.Id,
            Email = create.Email,
            NormalizedEmail = create.NormalizedEmail,
            Projects = create.Projects,
            Supervisor = create.Supervisor,
            EmailConfirmed = create.EmailConfirmed,
            PhoneNumber = create.PhoneNumber,
            LockoutEnabled = create.LockoutEnabled,
            LockoutEnd = create.LockoutEnd,
            UserName = create.UserName,
            ConcurrencyStamp = create.ConcurrencyStamp,
            PasswordHash = create.PasswordHash,
            SecurityStamp = create.SecurityStamp,
            AccessFailedCount = create.AccessFailedCount,
            NormalizedUserName = create.NormalizedUserName,
            PhoneNumberConfirmed = create.PhoneNumberConfirmed,
            TwoFactorEnabled = create.TwoFactorEnabled
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return new UserDetailsDTO(user.Id, user.UserName, user.NormalizedUserName, user.Email);
    }

    public async Task<Status> Update(int id, UserUpdateDTO update)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return Status.NotFound;
        }

        if (update.Name != user.UserName)
        {
            user.UserName = update.Name;
        }

        if (update.Email != user.Email)
        {
            user.Email = update.Email;
        }

        return Status.Updated;
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