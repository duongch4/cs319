namespace Web.API.Application.Models
{
    public class ResourceSkill
    {
        public int ResourceId {get; set;}
        public string ResourceDisciplineName { get; set; }
        public int SkillId {get; set;}
        public string Name {get; set;}

        public bool Equals(ResourceSkill other) {
            if(other is null) {
                return false;
            } else {
                return this.ResourceDisciplineName == other.ResourceDisciplineName && this.Name == other.Name;
            }
        }

        public override bool Equals(object obj) => Equals(obj as ResourceSkill);
        public override int GetHashCode() => (ResourceId, ResourceDisciplineName, Name).GetHashCode();

    }
}