using System.Collections.Generic;
using System.Threading.Tasks;
using Web.API.Application.Models;

namespace Web.API.Application.Repository
{
    public interface ISkillsRepository
    {
        //GET
        Task<IEnumerable<Skill>> GetAllSkills();
        Task<IEnumerable<Skill>> GetSkillsWithDiscipline(string disciplineName);
        Task<IEnumerable<Skill>> GetSkillsWithName(string skillName);
        Task<Skill> GetASkill(int skillId);
        //POST
        Task<Skill> CreateASkill(Skill skill);

        //PUT
        Task<Skill> UpdateASkill(Skill skill);

        //DELETE
        Task<Skill> DeleteASkill(int skillId); //TODO: Not sure if we can get skillId from just frontend?
    
        Task<IEnumerable<ResourceSkill>> GetUserSkills(int userId);

        Task<ResourceSkill> DeleteResourceSkill(ResourceSkill skill);
        Task<ResourceSkill> InsertResourceSkill(ResourceSkill skill);
        Task<int> GetSkillByDisciplineAndName(ResourceSkill skill);
    }
}
