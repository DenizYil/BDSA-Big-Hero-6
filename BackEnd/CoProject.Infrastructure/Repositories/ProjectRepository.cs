using CoProject.Infrastructure.DTOs;
using CoProject.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoProject.Infrastructure.Repositories;

public class ProjectRepository 
{
    private CoProjectContext context;

    public ProjectRepository(CoProjectContext _context)
    {
        context = _context;
    }

    public async Task<ProjectDTO?> Read(int id)
    {
        return await context.Projects.Where(p => p.Id == id).Select(p => new ProjectDTO
        (
            p.Id,
            p.Name,
            p.Description,
            p.Created,
            p.SupervisorId,
            p.Min,
            p.Max
        )).FirstOrDefaultAsync();
    }
    
    public async Task<IEnumerable<ProjectDTO>> ReadAll()
    {
        return await context.Projects.Select(p => new ProjectDTO(
            p.Id,
            p.Name,
            p.Description,
            p.Created,
            p.SupervisorId,
            p.Min,
            p.Max
        )).ToListAsync();
    }


}