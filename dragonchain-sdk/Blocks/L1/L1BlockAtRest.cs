﻿using System.Collections.Generic;

namespace DragonchainSDK.Blocks.L1
{    
    public class L1BlockAtRest
    {
        public string Version { get; set; }
        public string Dcrn { get; set; }
        public Header Header { get; set; }             
        public IEnumerable<string> Transactions { get; set; }
        public Common.Proof Proof { get; set; }
    }
}