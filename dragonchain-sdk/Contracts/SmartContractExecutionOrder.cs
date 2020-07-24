using Newtonsoft.Json;

namespace DragonchainSDK.Contracts
{
    public enum SmartContractExecutionOrder
    {
        [JsonProperty("parallel")]
        Parallel,
        [JsonProperty("serial")]
        Serial
    }
}