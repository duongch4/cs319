namespace Web.API.Resources
{
    public class LocationResource
    {
        /// <summary>Location Id</summary>
        /// <example>1</example>
        public int LocationID { get; set; }

        /// <summary>Location Province</summary>
        /// <example>British Columbia</example>
        public string Province { get; set; }
        
        /// <summary>Location City</summary>
        /// <example>Vancouver</example>
        public string City { get; set; }
    }

}
