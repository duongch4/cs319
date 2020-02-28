using System.Collections.Generic;
using Web.API.Resources;

namespace Web.API.Resources
{
    public class ProjectProfile
    {
        public ProjectSummary ProjectSummary { get; set; }
        public ProjectManager ProjectManager { get; set; }
        public IEnumerable<UserSummary> UsersSummary { get; set; }
        public IEnumerable<OpeningPositionsSummary> Openings { get; set; }
    }
}
