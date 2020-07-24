﻿using System.Collections.Generic;
using DragonchainSDK.Interchain.Networks;

namespace DragonchainSDK.Interchain.Bitcoin
{
    public class BitcoinTransactionRequest
    {
        public BitcoinNetwork Network { get; set; }
        public BitcoinTransaction Transaction { get; set; }
    }

    public class BitcoinTransaction
    {
        public decimal? Fee { get; set; }
        public string Data { get; set; }
        public string Change { get; set; }
        public IEnumerable<Output> Outputs { get; set; }
    }

    public class Output
    {
        public string To { get; set; }
        public decimal Value { get; set; }
    }
}