using NUnit.Framework;
using DragonchainSDK.Credentials;

namespace DragonchainSDK.Tests
{
    [TestFixture]
    public class CredentialServiceUnitTests
    {
        [Test]
        public void GetAuthorizationHeader_Test()
        {
            var credentialService = new CredentialService("testId", "key", "keyId");
            Assert.AreEqual("DC1-HMAC-SHA256 keyId:8Bc+h0parZxGeMB9rYzzRUuNxxHSIjGqSD4W/635A9k=",
                credentialService.GetAuthorizationHeader("GET", "/path", "timestamp", "application/json", ""));

            Assert.AreEqual("DC1-HMAC-SHA256 keyId:PkVjUxWZr6ST4xh+JxYFZresaFhQbk8sggWqyWv/XkU=",
                credentialService.GetAuthorizationHeader("POST", "/new_path", "timestamp", "application/json", "\"body\""));
        }
    }
}