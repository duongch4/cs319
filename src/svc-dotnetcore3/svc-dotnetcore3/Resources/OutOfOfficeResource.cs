using System;

namespace Web.API.Resources
{
    public class OutOfOfficeResource
    {        
        /// <summary>Absence Start Date</summary>
        /// <example>05/29/2021 05:50:06</example>
        public DateTime FromDate {get; set;}
        /// <summary>Project End Date</summary>
        /// <example>06/08/2021 05:50:06</example>
        public DateTime ToDate {get; set;}
        public string Reason {get; set;}
    }
}