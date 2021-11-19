using CoProject.Infrastructure.DTOs;
using CoProject.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoProject.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private CoProjectContext _context;

    public ProjectRepository(CoProjectContext context)
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
            })
            .ToListAsync();
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

        await _context.SaveChangesAsync();
        return Status.Updated;
    }
}