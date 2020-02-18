using Web.API.Application.Models;
using Web.API.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.API.Application.Repository
{
    public interface IProjectsRepository
    {
        // GET
        Task<IEnumerable<Project>> GetAllProjects();
        Task<IEnumerable<Project>> GetMostRecentProjects();
        Task<Project> GetAProject(string projectNumber);
        Task<ProjectResource> GetAProjectResource(string projectNumber);
        Task<IEnumerable<Project>> GetAllProjectsOfUser(User user);

        // POST
        Task<string> CreateAProject(ProjectProfile projectProfile, int locationId);

        // PUT
        Task<Project> UpdateAProject(Project project);

        // DELETE
        Task<Project> DeleteAProject(string number);
    }
}
