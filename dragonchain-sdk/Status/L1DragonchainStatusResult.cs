namespace DragonchainSDK.Status
{
    public class L1DragonchainStatusResult
    {
        /// <summary>
        /// Public ID of the Dragonchain
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Level of this dragonchain (as an integer)
        /// </summary>
        public string Level { get; set; }        
        /// <summary>
        /// URL of the chain
        /// </summary>
        public string Url { get; set; }        
        /// <summary>
        /// Proof scheme that this chain uses
        /// </summary>
        public string Scheme { get; set; }        
        /// <summary>
        /// Hashing algorithm used for blocks on this chain
        /// </summary>
        public string HashAlgo { get; set; }
        /// <summary>
        /// Dragonchain version of this chain
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Encryption algorithm used for blocks on this chain
        /// </summary>
        public string EncryptionAlgo { get; set; }
        /// <summary>
        /// Indicates whether indexing has been enabled for this chain
        /// </summary>
        public bool IndexingEnabled { get; set; }
    }
}