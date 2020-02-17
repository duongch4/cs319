namespace Web.API.Application.Models
{
    public class ResourceDisciplines
    {
        public int ResourceId {get; set;}
        public string DisciplineName { get; set; }
        public string YearsOfExperience {get; set;}

        public bool Equals(ResourceDisciplines other) {
            if(other is null) {
                return false;
            } else {
                return this.DisciplineName == other.DisciplineName && this.YearsOfExperience == other.YearsOfExperience;
            }
        }

        public override bool Equals(object obj) => Equals(obj as ResourceDisciplines);
        public override int GetHashCode() => (ResourceId, DisciplineName, YearsOfExperience).GetHashCode();
    }
}
