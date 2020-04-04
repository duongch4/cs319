using System;
using System.Collections.Generic;
using Web.API.Resources;

namespace Web.API.Application.Communication
{
    public class RequestProjectAssign
    {
        /// <summary>Opening ID</summary>
        /// <example>1</example>
        public int OpeningId { get; set; }
        
        /// <summary>User Id</summary>
        /// <example>"1"</example>
        public string UserID { get; set; }

        /// <summary>ConfirmedUtilization in percentage</summary>
        /// <example>90</example>
        public int ConfirmedUtilization { get; set; }
    }

    public class RequestUnassign
    {
        /// <summary>Id of position from which we unassigned a resource</summary>
        /// <example>941</example>
        public int OpeningId { get; set; }

        /// <summary>Id resource that was unassigned from position</summary>
        /// <example>15</example>
        public string UserId { get; set; }

        /// <summary>Updated Utilization of Resource that was removed</summary>
        /// <example>0</example>
        public int ConfirmedUtilization { get; set; }

        /// <summary>OpeningPositionsSummery of position from which we unassigned a resource</summary>
        /// <example>
        /// {
        ///    "positionID": 941,
        ///    "commitmentMonthlyHours": {
        ///        "2025-01-26": 110,
        ///        "2025-02-09": 58,
        ///        "2025-03-05": 65,
        ///        "2025-04-01": 102,
        ///        "2025-05-16": 114,
        ///        "2025-06-05": 179,
        ///        "2025-07-04": 123,
        ///        "2025-08-10": 100,
        ///        "2025-09-04": 30,
        ///        "2025-10-01": 140,
        ///        "2025-11-10": 175,
        ///        "2025-12-07": 187
        ///    },
        ///    "discipline": "Mining engineering‎",
        ///    "yearsOfExp": "10+",
        ///    "skills": []
        /// }
        /// </example>
        public OpeningPositionsSummary Opening { get; set; }
    }

    public class Utilization
    {
        public int Min { get; set; }
        public int Max { get; set; }
    }

    public class Filter
    {
        public Utilization Utilization { get; set; }
        public IEnumerable<LocationResource> Locations { get; set; }
        public Dictionary<string, IEnumerable<string>> Disciplines { get; set; }
        public IEnumerable<string> YearsOfExps { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class OrderKeyUser
    {
        /// <example>utilization</example>
        public string Value { get; set; }
        private OrderKeyUser(string value)
        {
            this.Value = value;
        }
        public static OrderKeyUser Utilization { get { return new OrderKeyUser("utilization"); } }
        public static OrderKeyUser Location { get { return new OrderKeyUser("location"); } }
        public static OrderKeyUser Discipline { get { return new OrderKeyUser("discipline"); } }
        public static OrderKeyUser YearsOfExp { get { return new OrderKeyUser("yearsOfExp"); } }
        public static OrderKeyUser StartDate { get { return new OrderKeyUser("startDate"); } }
        public static OrderKeyUser EndDate { get { return new OrderKeyUser("endDate"); } }
    }

    public class OrderKeyProject
    {
        /// <example>utilization</example>
        public string Value { get; set; }
        private OrderKeyProject(string value)
        {
            this.Value = value;
        }
        public static OrderKeyProject Title { get { return new OrderKeyProject("title"); } }
        public static OrderKeyProject StartDate { get { return new OrderKeyProject("startDate"); } }
        public static OrderKeyProject EndDate { get { return new OrderKeyProject("endDate"); } }
    }

    public class Order
    {
        /// <example>asc</example>
        public string Value { get; set; }
        private Order(string value)
        {
            this.Value = value;
        }
        public static Order Ascending { get { return new Order("asc"); } }
        public static Order Descending { get { return new Order("desc"); } }
    }

    public class RequestSearchUsers
    {
        /// <summary>Search Word for User's Last Name or First Name</summary>
        /// <example>Bour</example>
        public string SearchWord { get; set; }

        /// <summary>Filter Object</summary>
        public Filter Filter { get; set; }
        
        /// <summary>Order Key</summary>
        /// <example>utilization/province/city/discipline/yearsOfExp</example>
        public string OrderKey { get; set; }
        
        /// <summary>Order: Ascending/Descending</summary>
        /// <example>asc/desc</example>
        public string Order { get; set; }
        
        /// <summary>Page Number</summary>
        /// <example>1</example>
        public int Page { get; set; }
    }
}