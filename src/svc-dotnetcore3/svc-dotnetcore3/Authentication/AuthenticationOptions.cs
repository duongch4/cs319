﻿namespace Web.API.Authentication
{
    public class AuthenticationOptions
    {
        public string Authority { get; set; }
        public string AuthorizationUrl { get; set; }
        public string ClientId { get; set; }
        public string ApplicationIdUri { get; set; }
    }
}