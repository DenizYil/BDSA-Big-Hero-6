using CoProject.Infrastructure.Entities;
using CoProject.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;

namespace CoProject.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private ICoProjectContext _context;

    public ProjectRepository(ICoProjectContext context)
    {
        _context = context;
    }

    public async Task<ProjectDetailsDTO> Create(ProjectCreateDTO create)
    {
        var project = new Project
        {
            Name = create.Name,
            Description = create.Description,
            Created = DateTime.Now,
            SupervisorId = create.SupervisorId,
            Min = create.Min,
            Max = create.Max,
            Tags = await GetTagsFromNames(create.Tags),
            Users = new List<User>(),
            State = create.State
        };

        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();

        var supervisor = await _context.Users.FirstAsync(user => user.Id == create.SupervisorId);

        return new ProjectDetailsDTO(
            project.Id,
            project.Name,
            project.Description,
            new UserDetailsDTO(supervisor.Id, supervisor.Name, supervisor.Email, supervisor.Supervisor),
            project.State,
            project.Created,
            project.Tags.Select(tag => tag.Name).ToList(),
            new List<UserDetailsDTO>()
        )
        {
            Min = project.Min,
            Max = project.Max
        };
    }

    public async Task<ProjectDetailsDTO?> Read(int id)
    {
        return await _context.Projects
            .Where(p => p.Id == id)
            .Select(project =>
                new ProjectDetailsDTO(
                    project.Id,
                    project.Name,
                    project.Description,
                    _context.Users
                        .Where(user => user.Id == project.SupervisorId)
                        .Select(supervisor => new UserDetailsDTO(supervisor.Id, supervisor.Name, supervisor.Email, supervisor.Supervisor))
                        .First(),
                    project.State,
                    project.Created,
                    project.Tags.Select(tag => tag.Name).ToList(),
                    project.Users.Select(user => new UserDetailsDTO(user.Id, user.Name, user.Email, user.Supervisor)).ToList()
                )
                {
                    Min = project.Min,
                    Max = project.Max
                })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ProjectDetailsDTO>> ReadAll()
    {
        return await _context.Projects
            .Select(project =>
                new ProjectDetailsDTO(
                    project.Id,
                    project.Name,
                    project.Description,
                    _context.Users
                        .Where(user => user.Id == project.SupervisorId)
                        .Select(supervisor => new UserDetailsDTO(supervisor.Id, supervisor.Name, supervisor.Email, supervisor.Supervisor))
                        .First(),
                    project.State,
                    project.Created,
                    project.Tags.Select(tag => tag.Name).ToList(),
                    project.Users.Select(user => new UserDetailsDTO(user.Id, user.Name, user.Email, user.Supervisor)).ToList()
                )
                {
                    Min = project.Min,
                    Max = project.Max
                })
            .ToListAsync();
    }

    public async Task<Status> Update(int id, ProjectUpdateDTO update)
    {
        var project = await _context.Projects
            .Include(project => project.Tags)
            .Include(project => project.Users)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
        {
            return Status.NotFound;
        }

        if (update.Name != null && update.Name != project.Name)
        {
            project.Name = update.Name;
        }

        if (update.Description != null && update.Description != project.Description)
        {
            project.Description = update.Description;
        }

        if (update.Min != project.Min)
        {
            project.Min = update.Min;
        }

        if (update.Max != project.Max)
        {
            project.Max = update.Max;
        }

        if (update.State != null && update.State != project.State)
        {
            project.State = update.State.Value;
        }
        
        if (update.Tags != null)
        {
            foreach (var tag in project.Tags)
            {
                tag.Projects.Remove(project);
            }
            
            await _context.SaveChangesAsync();

            var tags = await GetTagsFromNames(update.Tags);
            
            project.Tags = tags;
        }

        if (update.Users != null)
        {
            foreach (var user in project.Users)
            {
                user.Projects.Remove(project);
            }
            
            await _context.SaveChangesAsync();

            project.Users = await _context.Users
                .Where(user => update.Users.Contains(user.Id))
                .ToListAsync();
        }

        await _context.SaveChangesAsync();
        return Status.Updated;
    }

    public async Task<Status> Delete(int id)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(project => project.Id == id);

        if (project == null)
        {
            return Status.NotFound;
        }

        project.State = State.Deleted;

        await _context.SaveChangesAsync();
        return Status.Deleted;
    }

    /*
     * HELPER METHODS
     */

    private async Task<ICollection<Tag>> GetTagsFromNames(IEnumerable<string> names)
    {
        var tags = new List<Tag>();

        foreach (var tagName in names)
        {
            var tag = await _context.Tags.FirstOrDefaultAsync(tag => tag.Name == tagName);

            if (tag == null)
            {
                tag = new Tag {Name = tagName};
                await _context.Tags.AddAsync(tag);
                await _context.SaveChangesAsync();
            }

            tags.Add(tag);
        }

        return tags;
    }
}