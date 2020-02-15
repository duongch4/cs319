using System.Collections.Generic;
using System.Threading.Tasks;
using Web.API.Application.Models;

namespace Web.API.Application.Repository
{
    //TODO
    public interface IDisciplinesRepository
    {
        //GET
        Task<Discipline> GetADiscipline(int disciplineID);
        Task<IEnumerable<Discipline>> GetAllDisciplines();
        Task<IEnumerable<Discipline>> GetDisciplinesByName(string disciplineName);
        Task<IEnumerable<User>> GetAllUsersWithDiscipline(string disciplineName);

        // POST
        Task<Discipline> CreateADiscipline(Discipline discipline);

        // PUT
        Task<Discipline> UpdateADiscipline(Discipline discipline);

        // DELETE
        Task<Discipline> DeleteADiscipline(Discipline discipline);
    }
}
