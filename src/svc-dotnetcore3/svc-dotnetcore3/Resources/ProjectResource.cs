using System.Collections.Generic;

namespace Web.API.Resources
{
    public class ProjectResource
    {
        // /// <summary>Project ID</summary>
        // /// <example>3570</example>
        // public int Id { get; set; }
        // /// <summary>Project Title</summary>
        // /// <example>Consequatur et assumenda cumque harum qui.</example>
        // public string TitleResource { get; set; }

        public ProjectDirectMappingResource ProjectSummary {get; set;}
        public IEnumerable<UserResource> UserSummary {get; set;}
        public IEnumerable<PositionResource> Positions {get; set;}
    }
}
