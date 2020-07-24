using Microsoft.Extensions.Configuration;
using DragonchainSDK.Framework.Errors;
using System;

namespace DragonchainSDK.Credentials.Manager
{
    public class CredentialManager : ICredentialManager
    {
        private readonly IConfiguration _config;        

        public CredentialManager(IConfiguration config)
        {
            _config = config;            
        }
        
        /// <summary>
        /// Get a dragonchainId from environment/config file
        /// </summary>  
        public string GetDragonchainId()
        {
            if (_config == null) { throw new FailureByDesignException(FailureCode.NOT_FOUND, "No configuration provider set"); }
            
            var defaultDragonchainIdIdentifier = "Dragonchain:DefaultDragonchainId";

            if (!string.IsNullOrEmpty(_config["default:dragonchain_id"]))
                defaultDragonchainIdIdentifier = "default:dragonchain_id";

            var id = _config[defaultDragonchainIdIdentifier];
            if (!string.IsNullOrWhiteSpace(id)) { return id; }
            throw new FailureByDesignException(FailureCode.NOT_FOUND, $"Config does not contain key '{defaultDragonchainIdIdentifier}'");
        }

        /// <summary>
        /// Get an a DragonchainCredentials object with AuthKey, AuthKeyId, and EndpointUrl
        /// </summary>   
        /// <param name="dragonchainId"> dragonchainId to get keys for</param>
        public DragonchainCredentials GetDragonchainCredentials(string dragonchainId = null)
        {   
            if (_config == null) { throw new FailureByDesignException(FailureCode.NOT_FOUND, "No configuration provider set"); }

            if (string.IsNullOrWhiteSpace(dragonchainId))
                dragonchainId = this.GetDragonchainId();

            var authKeyIdentifier = $"Dragonchain:Credentials:{dragonchainId}:AUTH_KEY";
            var authKeyIdIdentifier = $"Dragonchain:Credentials:{dragonchainId}:AUTH_KEY_ID";
            var endpointUrlIdentifier = $"Dragonchain:Credentials:{ dragonchainId}:ENDPOINT_URL";

            if (!string.IsNullOrEmpty(_config["default:dragonchain_id"]))
            {
                // config came from Dragonchain-standard ini-format credentials file
                authKeyIdentifier = $"{dragonchainId}:auth_key";
                authKeyIdIdentifier = $"{dragonchainId}:auth_key_id";
                endpointUrlIdentifier = $"{dragonchainId}:endpoint";
            }

            var authKey = _config[authKeyIdentifier];
            var authKeyId = _config[authKeyIdIdentifier];
            var endpointUrl = _config[endpointUrlIdentifier];
            if (!string.IsNullOrWhiteSpace(authKey) && !string.IsNullOrWhiteSpace(authKeyId) && !string.IsNullOrWhiteSpace(endpointUrl))
            {
                return new DragonchainCredentials { AuthKey = authKey, AuthKeyId = authKeyId, EndpointUrl = endpointUrl };
            }
            throw new FailureByDesignException(FailureCode.NOT_FOUND, $"Config does not contain keys '{authKeyIdentifier}', '{authKeyIdIdentifier}', and '{endpointUrlIdentifier}'");
        }
    }
}