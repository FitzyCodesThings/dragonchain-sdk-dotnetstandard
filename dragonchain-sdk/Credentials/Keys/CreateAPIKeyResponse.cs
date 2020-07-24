using Newtonsoft.Json;

namespace DragonchainSDK.Credentials.Keys
{
    public class CreateAPIKeyResponse
    {
        public string Key { get; set; }
        public string Id { get; set; }
        [JsonProperty("registration_time")]
        public int RegistrationTime { get; set; }
    }
}