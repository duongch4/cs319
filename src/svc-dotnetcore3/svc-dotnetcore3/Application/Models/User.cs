﻿namespace Web.API.Application.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public int LocationId { get; set; }
        public bool IsAdmin {get; set;}
        public bool IsManager {get; set;}
    }
}
