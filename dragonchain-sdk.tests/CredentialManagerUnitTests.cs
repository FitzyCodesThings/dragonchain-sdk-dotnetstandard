using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using dragonchain_sdk.Credentials.Manager;
using System.Collections.Generic;
using System;
using dragonchain_sdk.Framework.Errors;

namespace dragonchain_sdk.tests
{
    [TestFixture]
    public class CredentialManagerUnitTests
    {
        [Test]
        public void NullConfigTest()
        {
            var credentialManager = new CredentialManager(null);
            Assert.Throws<FailureByDesignException>(() => credentialManager.GetDragonchainId(), "No configuration provider set");
            Assert.Throws<FailureByDesignException>(() => credentialManager.GetDragonchainCredentials(), "No configuration provider set");
        }

        [Test]
        public void MissingCredentialsConfigTest()
        {
            var credentials = new Dictionary<string, string>
            {
                {"Dragonchain:DefaultDragonchainId", "fakeDragonchainId"},
                {"Dragonchain:Credentials:fakeDragonchainId:AUTH_KEY", "configAuthKey"},
                {"Dragonchain:Credentials:fakeDragonchainId:AUTH_KEY_ID", "configAuthKeyId"}
            };
            var config = new ConfigurationBuilder()                
                .AddInMemoryCollection(credentials)
                .Build();
            var credentialManager = new CredentialManager(config);
            Assert.Throws<FailureByDesignException>(() => credentialManager.GetDragonchainCredentials(), "Credential manager failed to throw due to missing credentials");            
        }

        [Test]
        public void InMemoryConfig_Test()
        {
            var credentials = new Dictionary<string, string>
            {
                {"Dragonchain:DefaultDragonchainId", "fakeDragonchainId"},
                {"Dragonchain:Credentials:fakeDragonchainId:AUTH_KEY", "configAuthKey"},
                {"Dragonchain:Credentials:fakeDragonchainId:AUTH_KEY_ID", "configAuthKeyId"},
                {"Dragonchain:Credentials:fakeDragonchainId:ENDPOINT_URL", "configEndpointURL"}
            };
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(credentials)
                .Build();

            var credentialManager = new CredentialManager(config: config);
            Assert.AreEqual("fakeDragonchainId", credentialManager.GetDragonchainId());

            var dragonchainCredentials = credentialManager.GetDragonchainCredentials();
            Assert.AreEqual("configAuthKeyId", dragonchainCredentials.AuthKeyId);
            Assert.AreEqual("configAuthKey", dragonchainCredentials.AuthKey);

            var fakeDragonchainIdDragonchainCredentials = credentialManager.GetDragonchainCredentials("fakeDragonchainId");
            Assert.AreEqual("configAuthKeyId", fakeDragonchainIdDragonchainCredentials.AuthKeyId);
            Assert.AreEqual("configAuthKey", fakeDragonchainIdDragonchainCredentials.AuthKey);
        }

        [Test]
        public void EnvironmentVariablesConfig_Test()
        {
            Environment.SetEnvironmentVariable("Dragonchain:DefaultDragonchainId", "fakeDragonchainId");
            Environment.SetEnvironmentVariable("Dragonchain:Credentials:fakeDragonchainId:AUTH_KEY", "configAuthKey");
            Environment.SetEnvironmentVariable("Dragonchain:Credentials:fakeDragonchainId:AUTH_KEY_ID", "configAuthKeyId");
            Environment.SetEnvironmentVariable("Dragonchain:Credentials:fakeDragonchainId:ENDPOINT_URL", "configEndpointUrl");
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var credentialManager = new CredentialManager(config: config);
            Assert.AreEqual("fakeDragonchainId", credentialManager.GetDragonchainId());

            var dragonchainCredentials = credentialManager.GetDragonchainCredentials();
            Assert.AreEqual("configAuthKeyId", dragonchainCredentials.AuthKeyId);
            Assert.AreEqual("configAuthKey", dragonchainCredentials.AuthKey);
            Assert.AreEqual("configEndpointUrl", dragonchainCredentials.EndpointUrl);
        }

        [Test]
        public void JsonConfig_Test()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var credentialManager = new CredentialManager(config: config);
            Assert.AreEqual("configTestId", credentialManager.GetDragonchainId());

            var dragonchainCredentials = credentialManager.GetDragonchainCredentials();
            Assert.AreEqual("configAuthKeyId", dragonchainCredentials.AuthKeyId);
            Assert.AreEqual("configAuthKey", dragonchainCredentials.AuthKey);
            Assert.AreEqual("configEndpointUrl", dragonchainCredentials.EndpointUrl);
        }

        [Test]
        public void XmlConfig_Test()
        {
            var config = new ConfigurationBuilder()
                .AddXmlFile("config.xml")
                .Build();

            var credentialManager = new CredentialManager(config: config);
            Assert.AreEqual("configTestId", credentialManager.GetDragonchainId());

            var dragonchainCredentials = credentialManager.GetDragonchainCredentials();
            Assert.AreEqual("configAuthKeyId", dragonchainCredentials.AuthKeyId);
            Assert.AreEqual("configAuthKey", dragonchainCredentials.AuthKey);
            Assert.AreEqual("configEndpointUrl", dragonchainCredentials.EndpointUrl);
        }

        [Test]
        public void IniConfig_Test()
        {
            var config = new ConfigurationBuilder()
                .AddIniFile("config.ini")
                .Build();

            var credentialManager = new CredentialManager(config: config);
            Assert.AreEqual("configTestId", credentialManager.GetDragonchainId());

            var dragonchainCredentials = credentialManager.GetDragonchainCredentials();
            Assert.AreEqual("configAuthKeyId", dragonchainCredentials.AuthKeyId);
            Assert.AreEqual("configAuthKey", dragonchainCredentials.AuthKey);
        }
    }
}

