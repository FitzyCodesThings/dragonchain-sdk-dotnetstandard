﻿namespace dragonchain_sdk.Blocks.L2
{
    public class L2BlockAtRest
    {
        public string Dcrn { get; set; }
        public Common.Proof Proof { get; set; }
        public string Version { get; set; }
        public Common.Header Header { get; set; }
        public Validation Validation { get; set; }        
    }
}