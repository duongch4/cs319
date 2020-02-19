using System.Collections.Generic;

namespace Web.API.Resources
{
    public class MasterResource
    {
        public Dictionary<string, IEnumerable<string>> disciplines { get; set; }
        public Dictionary<string, IEnumerable<string>> locations { get; set; }
        public IEnumerable<string> yearsOfExp { get; set; }
    }
}