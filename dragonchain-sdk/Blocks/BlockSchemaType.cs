﻿using System.Collections.Generic;
using Newtonsoft.Json;
using DragonchainSDK.Blocks.Common;

namespace DragonchainSDK.Blocks
{
    public class BlockSchemaType
    {
        public string Version { get; set; }
        public string Dcrn { get; set; }
        public BlockSchemaTypeHeader Header { get; set; }
        public BlockSchemaTypeProof Proof { get; set; }
        //L1
        public IEnumerable<string> Transactions { get; set; }
        //L2
        public L2.Validation Validation { get; set; }
        //L3
        [JsonProperty(PropertyName = "l2-Validations")]
        public L3.Validations L2Validations { get; set; }
        //L4
        [JsonProperty(PropertyName = "l3-Validations")]
        public IEnumerable<L4.Validations> L3Validations { get; set; }
        //L5
        [JsonProperty(PropertyName = "l4-blocks")]
        public IEnumerable<string> L4Blocks { get; set; }        
    }

    public class BlockSchemaTypeHeader : Header
    {
        //L1
        [JsonProperty(PropertyName = "prev_id")]
        public string PreviousId { get; set; }
        //L4
        [JsonProperty(PropertyName = "l1_block_id")]
        public int L1BlockId { get; set; }
        [JsonProperty(PropertyName = "l1_dc_id")]
        public int L1DcId { get; set; }
        [JsonProperty(PropertyName = "l1_proof")]
        public int L1Proof { get; set; }
    }

    public class BlockSchemaTypeProof : Proof
    {
        //L5
        [JsonProperty(PropertyName = "transaction_hash")]
        public string TransactionHash { get; set; }
        [JsonProperty(PropertyName = "block_last_sent_at")]
        public string BlockLastSentAt { get; set; }        
        public string Network { get; set; }
    }
}