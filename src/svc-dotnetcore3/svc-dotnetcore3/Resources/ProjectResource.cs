using System;
using Web.API.Application.Models;

namespace Web.API.Resources
{
    public class ProjectResource
    {
        /// <summary>Max Pages from A Search</summary>
        /// <example>10</example>
        public int MaxPages { get; set; }
        
        /// <summary>Project Id</summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>Project Number</summary>
        /// <example>2009-VD9D-15</example>
        public string Number { get; set; }

        /// <summary>Project Manager Id</summary>
        /// <example>"1"</example>
        public string ManagerId { get; set; }

        /// <summary>Project Manager First Name</summary>
        /// <example>Jason</example>
        public string FirstName { get; set; }

        /// <summary>Project Manager Last Name</summary>
        /// <example>Bourne</example>
        public string LastName { get; set; }

        /// <summary>Project Title</summary>
        /// <example>Designing the new Pawnee Commons</example>
        public string Title { get; set; }

        /// <summary>Location Id</summary>
        /// <example>5</example>
        public int LocationId { get; set; }

        /// <summary>Location Province</summary>
        /// <example>Ontario</example>
        public string Province { get; set; }

        /// <summary>Location City</summary>
        /// <example>Toronto</example>
        public string City { get; set; }

        /// <summary>Project Start Date</summary>
        /// <example>05/29/2021 05:50:06</example>
        public DateTime ProjectStartDate { get; set; }

        /// <summary>Project End Date</summary>
        /// <example>01/29/2022 05:50:06</example>
        public DateTime ProjectEndDate { get; set; }
    }
}