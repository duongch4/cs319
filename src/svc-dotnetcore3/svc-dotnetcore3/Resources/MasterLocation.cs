using System.Collections.Generic;

namespace Web.API.Resources
{
    public class MasterLocation
    {
        /// <summary>Province</summary>
        /// <example>Ontario</example>
        public string Province { get; set; }

        /// <summary>[City--Id] pairs</summary>
        /// <example>String that lists all [city--id] pairs separated by comma: "Waterloo--1,Toronto--2"</example>
        public string CitiesIds { get; set; }
    }
}