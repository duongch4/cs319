using System;
using System.Collections.Generic;

namespace Web.API.Application.Models
{
    public class Availability
    {
        public IEnumerable<DateTime> fromDateTimes;
        public IEnumerable<DateTime> toDateTimes;
    }
}
