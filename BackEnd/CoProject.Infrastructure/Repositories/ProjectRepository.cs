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
    
    public async Task<(Status, int)> Create(ProjectCreateDTO create)
    {
           
    }

    public async Task<Status> Delete(int id)
    {
        
        // TODO: tjek om den findes
    }

    public async Task<(Status, ProjectDTO)> Update(ProjectUpdateDTO update)
    {
        
        // todo: tjek om den findes
        
    }
}