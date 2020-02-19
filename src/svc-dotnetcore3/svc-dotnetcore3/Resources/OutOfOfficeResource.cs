using System;

namespace Web.API.Resources
{
    public class OutOfOfficeResource
    {        
        /// <summary>Absence Start Date</summary>
        /// <example>2021-05-29T05:50:06.0000000</example>
        public DateTime FromDate {get; set;}
        /// <summary>Project End Date</summary>
        /// <example>2222-01-29T05:50:06.0000000</example>
        public DateTime ToDate {get; set;}
        /// <summary>Reason for unavailability</summary>
        /// <example>Died on mission</example>
        public string Reason {get; set;}
    }
}