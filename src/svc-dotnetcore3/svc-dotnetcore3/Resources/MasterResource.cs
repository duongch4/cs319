using System.Collections.Generic;

namespace Web.API.Resources
{
    public class MasterResource
    {
        public Dictionary<string, IEnumerable<string>> Disciplines { get; set; }
        public Dictionary<string, IEnumerable<string>> Locations { get; set; }
        public IEnumerable<string> YearsOfExp { get; set; }
    }
}