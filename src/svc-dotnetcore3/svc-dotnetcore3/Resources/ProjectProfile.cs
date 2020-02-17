using System.Collections.Generic;

namespace Web.API.Resources
{
    public class ProjectProfile
    {
        public ProjectSummary ProjectSummary {get; set;}
        public IEnumerable<UserResource> UserSummary {get; set;}
        public IEnumerable<PositionResource> Positions {get; set;}
    }
}
