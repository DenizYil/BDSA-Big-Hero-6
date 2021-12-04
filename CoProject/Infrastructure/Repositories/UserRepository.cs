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

    public async Task<UserDetailsDTO?> Read(string id)
    {
        return await _context.Users
            .Where(u => u.Id == id)
            .Select(u => new UserDetailsDTO(u.Id, u.UserName, u.Email))
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserDetailsDTO>> ReadAll()
    {
        return await _context.Users
            .Select(u => new UserDetailsDTO(u.Id, u.UserName, u.Email))
            .ToListAsync();
    }

    public async Task<IEnumerable<ProjectDetailsDTO>> ReadAllByUser(string id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);

        if (user == null)
        {
            return new List<ProjectDetailsDTO>();
        }

        return user.Projects
            .Select(project =>
                new ProjectDetailsDTO(
                    project.Id,
                    project.Name,
                    project.Description,
                    project.SupervisorId,
                    project.State,
                    project.Created,
                    project.Tags.Select(tag => tag.Name).ToList(),
                    project.Users.Select(u => new UserDetailsDTO(u.Id, u.UserName, u.Email))
                        .ToList()
                )
                {
                    Min = project.Min,
                    Max = project.Max
                })
            .ToList();
    }

    public async Task<UserDetailsDTO> Create(UserCreateDTO create)
    {
        var user = new User
        {
            Id = create.Id,
            UserName = create.Name,
            Email = create.Email,
            Supervisor = create.Supervisor,
            Projects = new List<Project>()
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return new UserDetailsDTO(user.Id, user.UserName, user.Email);
    }

    public async Task<Status> Update(string id, UserUpdateDTO update)
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

    public async Task<Status> Delete(string id)
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