namespace DragonchainSDK.Credentials.Manager
{
    public interface ICredentialManager
    {
        DragonchainCredentials GetDragonchainCredentials(string dragonchainId = null);        
        string GetDragonchainId();
    }
}