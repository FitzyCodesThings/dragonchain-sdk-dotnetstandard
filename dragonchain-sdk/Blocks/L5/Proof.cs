﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace DragonchainSDK.Blocks.L5
{
    public class Proof : Common.Proof
    {        
        [JsonProperty(PropertyName = "transaction_hash")]
        public IEnumerable<string> TransactionHash { get; set; }
        [JsonProperty(PropertyName = "block_last_sent_at")]
        public string BlockLastSentAt { get; set; }
        public string Network { get; set; }
    }
}
