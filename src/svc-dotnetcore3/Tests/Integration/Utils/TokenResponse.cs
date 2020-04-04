using Newtonsoft.Json;

namespace Tests.Integration.Utils
{
    class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
