using Web.API.Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.API.Application.Repository
{
    public interface IPositionsRepository
    {
        // GET
        Task<IEnumerable<Position>> GetPositionsOfUser(User user);
        Task<IEnumerable<Position>> GetAllUnassignedPositionOfProject(Project project);
        Task<Project> GetAProject(string projectNumber);
        Task<IEnumerable<Project>> GetAllProjectsOfUser(User user);

        // POST
        Task<Project> CreateAProject(Project project);

        // PUT
        Task<Project> UpdateAProject(Project project);

        // DELETE
        Task<Project> DeleteAProject(string number);
    }
}
