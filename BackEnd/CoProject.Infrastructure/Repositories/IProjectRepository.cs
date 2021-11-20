using CoProject.Infrastructure.DTOs;

namespace CoProject.Infrastructure.Repositories;

public interface IProjectRepository : IRepository
{
    Task<ProjectDetailsDTO?> Read(int id);
    Task<IEnumerable<ProjectDetailsDTO>> ReadAll();
    Task<(Status, int)> Create(ProjectCreateDTO create);
    Task<Status> Update(ProjectUpdateDTO update);
    Task<Status> Delete(int id);
}