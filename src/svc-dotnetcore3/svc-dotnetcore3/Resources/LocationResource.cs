namespace Web.API.Resources
{
    public class LocationResource
    {
        /// <summary>Location ID</summary>
        /// <example>5</example>
        public int Id { get; set; }
        /// <summary>Location Code</summary>
        /// <example>edm</example>
        public string Code { get; set; }
        /// <summary>Location Name</summary>
        /// <example>Edmonton</example>
        public string Name { get; set; }
    }
}
