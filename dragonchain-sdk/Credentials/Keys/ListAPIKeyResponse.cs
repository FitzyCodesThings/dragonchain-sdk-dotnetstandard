using System.Collections.Generic;

namespace DragonchainSDK.Credentials.Keys
{
    public class ListAPIKeyResponse
    {
        public IEnumerable<GetAPIKeyResponse> Keys { get; set; }
    }
}