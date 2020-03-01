using System;
using System.Collections.Generic;
using Web.API.Resources;

namespace Web.API.Application.Communication
{
    public class RequestProjectAssign
    {
        /// <summary>Position ID</summary>
        /// <example>1</example>
        public int PositionID { get; set; }
        
        /// <summary>User ID</summary>
        /// <example>1</example>
        public int UserID { get; set; }
    }

    public class Filter
    {
        public IEnumerable<int> Utilizations { get; set; }
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
        /// <summary>Filter Object</summary>
        public Filter Filter { get; set; }
        
        /// <summary>Order Key</summary>
        public string OrderKey { get; set; }
        
        /// <summary>Order: Ascending/Descending</summary>
        public string Order { get; set; }
        
        /// <summary>Page Number</summary>
        /// <example>1</example>
        public int Page { get; set; }
    }
}