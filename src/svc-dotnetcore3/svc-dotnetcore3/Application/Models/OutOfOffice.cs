using System;

namespace Web.API.Application.Models
{
    public class OutOfOffice
    {
        public int UserId {get; set;}
        public DateTime FromDateTime {get; set;}
        public DateTime ToDateTime {get; set;}
    }
}
