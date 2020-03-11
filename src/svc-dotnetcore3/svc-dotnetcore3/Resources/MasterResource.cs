using System.Collections.Generic;

namespace Web.API.Resources
{
    public class MasterResource
    {
        public Dictionary<string, MasterDiscipline> Disciplines { get; set; }
        public Dictionary<string, Dictionary<string, int>> Locations { get; set; }
        public IEnumerable<string> YearsOfExp { get; set; }
    }
}