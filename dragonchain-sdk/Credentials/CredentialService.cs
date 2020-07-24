using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using DragonchainSDK.Credentials.Manager;

namespace DragonchainSDK.Credentials
{
    /// <summary>
    /// Service to retrieve Dragonchain credentials for use in API requests
    /// </summary>
    public class CredentialService : ICredentialService
    {
        private readonly ILogger _logger;        
        private DragonchainCredentials _credentials;
        private HmacAlgorithm _hmacAlgo;

        public string DragonchainId { get; }

        /// <summary>
        /// Create an Instance of a CredentialService
        /// </summary>
        /// <param name="dragonchainId">dragonchainId associated with these credentials</param>
        /// <param name="authKey">authKey to use with these credentials</param>
        /// <param name="authKeyId">authKeyId to use with these credentials</param>
        /// <param name="hmacAlgo">hmac algorithm to use</param>
        /// <param name="credentialManager">manager to retrieve Dragonchain credentials from config provider</param>
        /// <param name="logger"></param>
        public CredentialService(string dragonchainId, string authKey = "", string authKeyId = "", string endpointUrl = "", HmacAlgorithm hmacAlgo = HmacAlgorithm.SHA256, ICredentialManager credentialManager = null, ILogger<DragonchainClient> logger = null)
        {
            _logger = logger ?? new NullLogger<DragonchainClient>();
            DragonchainId = dragonchainId;
            if (!string.IsNullOrWhiteSpace(authKey) && !string.IsNullOrWhiteSpace(authKeyId))
            {
                _logger.LogDebug("Auth Key/Id and endpoint URL provided explicitly, will not search env/disk");
                _credentials = new DragonchainCredentials{ AuthKey  = authKey, AuthKeyId = authKeyId, EndpointUrl = endpointUrl };
            }
            else
            {
                try
                {
                    _credentials = credentialManager.GetDragonchainCredentials(dragonchainId);
                }
                catch
                {  // don't require credentials to be present on construction
                    _credentials = new DragonchainCredentials { AuthKey = string.Empty, AuthKeyId = string.Empty, EndpointUrl = string.Empty };
                }
            }
            _hmacAlgo = hmacAlgo;
        }        

        /// <summary>
        /// Manually override the credentials for this instance
        /// </summary>                
        public void OverrideCredentials(string authKeyId, string authKey, string endpointUrl)
        {
            _credentials = new DragonchainCredentials { AuthKey = authKey, AuthKeyId = authKeyId, EndpointUrl = endpointUrl };
        }

        /// <summary>
        /// Return the HMAC signature used as the Authorization Header on REST requests to your dragonchain.
        /// </summary>        
        public string GetAuthorizationHeader(string method, string path, string timestamp, string contentType, string body)
        {
            var hmac = CreateHmac(_hmacAlgo, _credentials.AuthKey);
            var message = GetMessageString(method, path, DragonchainId, timestamp, contentType, body);            
            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(message)));
            return $"DC1-HMAC-{_hmacAlgo.ToValue()} {_credentials.AuthKeyId}:{signature}";
        }

        private HashAlgorithm CreateHmac(HmacAlgorithm hmacAlgo, string authKey)
        {
            var authKeyBytes = Encoding.UTF8.GetBytes(authKey);
            switch (hmacAlgo)
            {
                case HmacAlgorithm.BLAKE2b512:
                case HmacAlgorithm.SHA3_256:
                    throw new NotSupportedException();
                default:
                    return new HMACSHA256(authKeyBytes);
            }
        }

        private string GetMessageString(string method, string path, string dragonchainId, string timestamp, string contentType, string body)
        {            
            var sha256 = SHA256.Create();
            var binaryBody = string.IsNullOrWhiteSpace(body) ? Encoding.UTF8.GetBytes("") : Encoding.UTF8.GetBytes(body);            
            var hashedBase64Content = Convert.ToBase64String(sha256.ComputeHash(binaryBody));
            return String.Join("\n", new string[] { method.ToUpper(), path, dragonchainId, timestamp, contentType, hashedBase64Content });            
        }

        public string GetEndpointURL()
        {
            return _credentials.EndpointUrl;
        }
    }
}

/**
 * All Humans are welcome.
 */
