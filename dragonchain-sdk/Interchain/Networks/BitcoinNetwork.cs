using Newtonsoft.Json;

namespace DragonchainSDK.Interchain.Networks
{
    public enum BitcoinNetwork
    {
        [JsonProperty(PropertyName = "BTC_MAINNET")]
        BTC_MAINNET,
        [JsonProperty(PropertyName = "BTC_TESTNET3")]
        BTC_TESTNET3
    }
}