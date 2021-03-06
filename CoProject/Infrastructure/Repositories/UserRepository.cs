namespace CoProject.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ICoProjectContext _context;

    public UserRepository(ICoProjectContext context)
    {
        _context = context;
    }

    public async Task<UserDetailsDTO?> Read(string id)
    {
        return await _context.Users
            .Where(u => u.Id == id)
            .Select(u => new UserDetailsDTO(u.Id, u.Name, u.Email, u.Supervisor, u.Image))
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserDetailsDTO>> ReadAll()
    {
        return await _context.Users
            .Select(u => new UserDetailsDTO(u.Id, u.Name, u.Email, u.Supervisor, u.Image))
            .ToListAsync();
    }

    public async Task<IEnumerable<ProjectDetailsDTO>> ReadAllByUser(string id)
    {
        var user = await _context.Users
            .Include(user => user.Projects)
            .ThenInclude(project => project.Tags)
            .FirstOrDefaultAsync(user => user.Id == id);

        if (user == null)
        {
            return new List<ProjectDetailsDTO>();
        }

        if (user.Supervisor)
        {
            return _context.Projects
                .Where(project => project.SupervisorId == user.Id)
                .Select(project => new ProjectDetailsDTO(
                    project.Id,
                    project.Name,
                    project.Description,
                    new(user.Id, user.Name, user.Email, user.Supervisor, user.Image),
                    project.State,
                    project.Created,
                    project.Tags
                        .Select(tag => tag.Name)
                        .ToList(),
                    project.Users
                        .Select(u => new UserDetailsDTO(u.Id, u.Name, u.Email, u.Supervisor, u.Image))
                        .ToList()
                )
                {
                    Min = project.Min,
                    Max = project.Max
                });
        }

        return user.Projects
            .Select(project =>
                new ProjectDetailsDTO(
                    project.Id,
                    project.Name,
                    project.Description,
                    _context.Users
                        .Where(supervisor => supervisor.Id == project.SupervisorId)
                        .Select(supervisor => new UserDetailsDTO(supervisor.Id, supervisor.Name, supervisor.Email, supervisor.Supervisor, supervisor.Image))
                        .First(),
                    project.State,
                    project.Created,
                    project.Tags
                        .Select(tag => tag.Name)
                        .ToList(),
                    project.Users
                        .Select(u => new UserDetailsDTO(u.Id, u.Name, u.Email, u.Supervisor, u.Image))
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
            Name = create.Name,
            Email = create.Email,
            Supervisor = create.Supervisor,
            Projects = new List<Project>()
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return new(user.Id, user.Name, user.Email, user.Supervisor, user.Image);
    }

    public async Task<Status> Update(string id, UserUpdateDTO update)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return Status.NotFound;
        }

        if (update.Name != user.Name)
        {
            user.Name = update.Name;
        }

        if (update.Email != user.Email)
        {
            user.Email = update.Email;
        }

        if (update.Image != null && update.Image.Path != null)
        {
            user.Image = update.Image.Path;
        }

        if (update.Supervisor != null)
        {
            user.Supervisor = update.Supervisor.Value;
        }

        await _context.SaveChangesAsync();
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