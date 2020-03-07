using System.Collections.Generic;
using System.Threading.Tasks;
using Web.API.Application.Models;
using Web.API.Resources;

namespace Web.API.Application.Repository
{
    //TODO
    public interface IDisciplinesRepository
    {
        //GET
        Task<Discipline> GetADiscipline(int disciplineId);
        Task<IEnumerable<Discipline>> GetAllDisciplines();
        Task<IEnumerable<DisciplineResource>> GetAllDisciplinesWithSkills();
        Task<IEnumerable<Discipline>> GetDisciplinesByName(string disciplineName);

        // POST
        Task<Discipline> CreateADiscipline(Discipline discipline);

        // PUT
        Task<Discipline> UpdateADiscipline(Discipline discipline);

        // DELETE
        Task<Discipline> DeleteADiscipline(int disciplineId);

        Task<IEnumerable<ResourceDiscipline>> GetUserDisciplines(int userId);
        Task<ResourceDiscipline> DeleteResourceDiscipline(ResourceDiscipline discipline);
        Task<ResourceDiscipline> InsertResourceDiscipline(ResourceDiscipline discipline);
    }
}
