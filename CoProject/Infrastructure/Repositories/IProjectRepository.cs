namespace CoProject.Infrastructure.Repositories;

public interface IProjectRepository
{
    Task<ProjectDetailsDTO?> Read(int id);
    Task<IEnumerable<ProjectDetailsDTO>> ReadAll();
    Task<ProjectDetailsDTO> Create(ProjectCreateDTO create);
    Task<Status> Update(int id, ProjectUpdateDTO update);
    Task<Status> Delete(int id);
}