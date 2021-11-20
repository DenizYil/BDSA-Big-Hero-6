using CoProject.Infrastructure.DTOs;
using CoProject.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoProject.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private ICoProjectContext _context;

    public ProjectRepository(ICoProjectContext context)
    {
        _context = context;
    }

    public async Task<ProjectDTO?> Read(int id)
    {
        return await _context.Projects
            .Where(p => p.Id == id)
            .Select(p => new ProjectDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Created = p.Created,
                SupervisorId = p.SupervisorId,
                Min = p.Min,
                Max = p.Max,
                State = p.State,
                Tags = p.Tags.Select(tag => tag.Name).ToList(),
                Users = p.Users.Select(user => new UserDTO
                {
                    Name = user.NormalizedUserName,
                    UserName = user.UserName,
                    Email = user.Email
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ProjectDTO>> ReadAll()
    {
        return await _context.Projects
            .Select(p => new ProjectDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Created = p.Created,
                SupervisorId = p.SupervisorId,
                Min = p.Min,
                Max = p.Max,
                State = p.State,
                Tags = p.Tags.Select(tag => tag.Name).ToList(),
                Users = p.Users.Select(user => new UserDTO
                {
                    Name = user.NormalizedUserName,
                    UserName = user.UserName,
                    Email = user.Email
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<(Status, int)> Create(ProjectCreateDTO create)
    {
        var project = new Project{
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
        return (Status.Created, project.Id);
    }
    
    public async Task<Status> Update(ProjectUpdateDTO update)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == update.Id);

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

        if (update.Tags != null && update.Tags.All(project.Tags.Select(tag => tag.Name).Contains))
        {
            project.Tags = await GetTagsFromNames(update.Tags);
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

    private async Task<IReadOnlyCollection<Tag>> GetTagsFromNames(IEnumerable<string> names)
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