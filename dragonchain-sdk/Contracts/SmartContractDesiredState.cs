using Newtonsoft.Json;

namespace DragonchainSDK.Contracts
{
    public enum SmartContractDesiredState
    {
        [JsonProperty("active")]
        Active,
        [JsonProperty("inactive")]
        Inactive
    }
}