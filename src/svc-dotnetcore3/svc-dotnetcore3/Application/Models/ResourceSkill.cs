namespace Web.API.Application.Models
{
    public class ResourceSkill
    {
        public int ResourceId {get; set;}
        public int ResourceDisciplineId { get; set; }
        public int SkillDisciplineId {get; set;}
        public int SkillId {get; set;}
        public string Name {get; set;}

    }
}