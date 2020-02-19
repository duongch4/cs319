using System.Collections.Generic;

namespace Web.API.Resources
{
    public class ProjectProfile
    {
        public ProjectSummary ProjectSummary { get; set; }
        public IEnumerable<UserSummary> UsersSummary { get; set; }
        public IEnumerable<OpeningPositionsSummary> Openings { get; set; }
    }
}
