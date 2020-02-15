using System;

namespace Web.API.Application.Models
{
    public class OutOfOffice
    {
        public int ResourceId {get; set;}
        public DateTime FromDate {get; set;}
        public DateTime ToDate {get; set;}
    }
}
