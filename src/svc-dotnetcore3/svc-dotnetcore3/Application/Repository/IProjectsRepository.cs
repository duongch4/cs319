using Web.API.Application.Models;
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

        // POST
        Task<Project> CreateAProject(Project project);

        // PUT
        Task<Project> UpdateAProject(Project project);

        // DELETE
        Task<Project> DeleteAProject(string number);
    }
}
