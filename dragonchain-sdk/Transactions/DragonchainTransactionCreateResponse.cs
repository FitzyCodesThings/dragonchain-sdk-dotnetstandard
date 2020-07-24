using Newtonsoft.Json;

namespace DragonchainSDK.Transactions
{
    public class DragonchainTransactionCreateResponse
    {
        [JsonProperty(PropertyName = "transaction_id")]        
        public string TransactionId { get; set; }
    }
}