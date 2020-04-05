using Web.API.Application.Models;
using Web.API.Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.API.Application.Repository
{
    public interface IProjectsRepository
    {
        // GET
        Task<IEnumerable<ProjectResource>> GetAllProjects();
        Task<IEnumerable<string>> GetAllProjectNumbersOfManager(string managerId);
        Task<IEnumerable<ProjectResource>> GetAllProjectResources(string orderKey, string order, int page, int rowsPerPage);
        Task<IEnumerable<ProjectResource>> GetAllProjectResourcesWithTitle(string searchWord, string orderKey, string order, int page, int rowsPerPage);
        Task<IEnumerable<ProjectResource>> GetAllProjectResourcesOfUser(string userId);
        Task<IEnumerable<Project>> GetMostRecentProjects();
        Task<Project> GetAProject(string projectNumber);
        Task<ProjectResource> GetAProjectResource(string projectNumber);

        // POST
        Task<string> CreateAProject(ProjectProfile projectProfile, int locationId);

        // PUT
        Task<string> UpdateAProject(ProjectProfile projectProfile, int locationId);

        // DELETE
        Task<int> DeleteAProject(string number);
    }
}
