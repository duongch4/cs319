using System.Collections.Generic;

namespace Web.API.Resources
{
    public class MasterResource
    {
        public Dictionary<string, string[]> disciplines { get; set; }
        public Dictionary<string, List<string>> locations { get; set; }
        public IEnumerable<string> yearsOfExp { get; set; }
    }
}