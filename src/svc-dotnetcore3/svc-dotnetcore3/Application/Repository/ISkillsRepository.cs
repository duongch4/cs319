using System.Collections.Generic;
using System.Threading.Tasks;
using Web.API.Application.Models;
using Web.API.Resources;

namespace Web.API.Application.Repository
{
    public interface ISkillsRepository
    {
        //GET
        Task<IEnumerable<Skill>> GetAllSkills();
        Task<IEnumerable<Skill>> GetSkillsWithDiscipline(string disciplineName);
        Task<IEnumerable<Skill>> GetSkillsWithName(string skillName);
        Task<Skill> GetASkill(int skillId);
        Task<Skill> GetASkill(string skillName, int disciplineId);
        //POST
        Task<Skill> CreateASkill(Skill skill);
        Task<int> CreateASkill(DisciplineSkillResource skill);

        //PUT
        Task<Skill> UpdateASkill(Skill skill);

        //DELETE
        Task<Skill> DeleteASkill(int skillId); //TODO: Not sure if we can get skillId from just frontend?
        Task<Skill> DeleteASkill(string skillName, int disciplineId);

        Task<IEnumerable<ResourceSkill>> GetUserSkills(int userId);

        Task<ResourceSkill> DeleteResourceSkill(ResourceSkill skill);
        Task<ResourceSkill> InsertResourceSkill(ResourceSkill skill);
        Task<int> GetSkillByDisciplineAndName(ResourceSkill skill);
    }
}
