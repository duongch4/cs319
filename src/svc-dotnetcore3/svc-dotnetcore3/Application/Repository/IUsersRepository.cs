using System.Collections.Generic;
using System.Threading.Tasks;
using Web.API.Application.Models;
using Web.API.Resources;

namespace Web.API.Application.Repository
{
    public interface IUsersRepository
    {
        //GET
        Task<IEnumerable<User>> GetAllUsers();
        Task<User> GetAUser(int userId);
        Task<IEnumerable<User>> GetAllUsersAtLocation(Location location);
        Task<IEnumerable<User>> GetAllUsersWithDiscipline(Discipline discipline);
        Task<IEnumerable<User>> GetAllUsersWithSkill(Skill skill);

        // TODO:
        // Task<IEnumerable<User>> GetAllUsersWithAvailability(Availability requestedAvailability);
        // Task<IEnumerable<User>> GetAllUsersOverNUtilization(int nUtil);
        Task<IEnumerable<User>> GetAllUsersOnProject(Project project);
        Task<IEnumerable<UserResource>> GetAllUsersResourceOnProject(int projectId, int projectManagerId);

        // TODO:
        // Task<IEnumerable<User>> GetAllUsersWithYearsOfExp(Discipline discipline, int yrsOfExp);
        Task<User> GetPMOfProject(Project project);

        //PUT
        Task<int> UpdateAUser(UserSummary user, Location location);
        Task<IEnumerable<UserResource>> GetAllUsersGeneral();
    }
}
