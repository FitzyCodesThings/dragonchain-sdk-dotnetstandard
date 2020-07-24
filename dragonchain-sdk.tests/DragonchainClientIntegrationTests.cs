﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using DragonchainSDK.Blocks;
using DragonchainSDK.Contracts;
using DragonchainSDK.Credentials;
using DragonchainSDK.Framework.Web;
using DragonchainSDK.Status;
using DragonchainSDK.Transactions;
using DragonchainSDK.Transactions.Bulk;
using DragonchainSDK.Transactions.L1;
using DragonchainSDK.Transactions.Types;

namespace DragonchainSDK.Tests
{
    [TestFixture]
    public class DragonchainClientIntegrationTests
    {           
        private DragonchainClient _dragonchainLevel1Client;
        private DragonchainClient _dragonchainLevel2Client;

        public DragonchainClientIntegrationTests()
        {            
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<DragonchainClientIntegrationTests>();

            var configuration = builder.Build();

            //dotnet user-secrets set LevelOneId <id>
            var levelOneId = configuration["LevelOneId"];
            //dotnet user-secrets set LevelOneAuthKeyId <id>
            var levelOneAuthKeyId = configuration["LevelOneAuthKeyId"];
            //dotnet user-secrets set LevelOneAuthKey <key>
            var levelOneAuthKey = configuration["LevelOneAuthKey"];

            //dotnet user-secrets set LevelTwoId <id>
            var levelTwoId = configuration["LevelTwoId"];
            //dotnet user-secrets set LevelTwoAuthKeyId <keyId>
            var levelTwoAuthKeyId = configuration["LevelTwoAuthKeyId"];
            //dotnet user-secrets set LevelTwoAuthKey <key>
            var levelTwoAuthKey = configuration["LevelTwoAuthKey"];

            var logger = new TestLogger<DragonchainClient>();
            if (AreRequiredUserSecretsAvailable(levelOneId, levelOneAuthKey, levelOneAuthKeyId))
            {
                var level1credentialService = new CredentialService(levelOneId, levelOneAuthKey, levelOneAuthKeyId);
                _dragonchainLevel1Client = new DragonchainClient(levelOneId, credentialManager: null, credentialService: level1credentialService, logger: logger);                
            }
            if (AreRequiredUserSecretsAvailable(levelTwoId, levelTwoAuthKey, levelTwoAuthKey))
            {
                var level2CredentialService = new CredentialService(levelTwoId, levelTwoAuthKey, levelTwoAuthKeyId);
                _dragonchainLevel2Client = new DragonchainClient(levelTwoId, credentialManager: null, credentialService: level2CredentialService, logger: logger);
            }
        }
        
        [Test]
        public async Task Transaction_Tests()
        {            
            if (AreLevel1TestsConfigured())
            {
                // register transaction type test
                var transactionType = "test";
                var registerTransactionTypeResult = await _dragonchainLevel1Client.CreateTransactionType(transactionType, new List<TransactionTypeCustomIndex>
                    {
                        new TransactionTypeCustomIndex{ Key ="TestKey", Path="TestPath" }
                    }
                );
                Assert.AreEqual(200, registerTransactionTypeResult.Status);
                Assert.IsTrue(registerTransactionTypeResult.Ok);
                Assert.IsInstanceOf<TransactionTypeSimpleResponse>(registerTransactionTypeResult.Response);

                try
                {
                    // update transaction type test
                    var updateTransactionTypeResult = await _dragonchainLevel1Client.UpdateTransactionType(transactionType, new List<TransactionTypeCustomIndex>
                    {
                        new TransactionTypeCustomIndex{ Key ="NewTestKey", Path="NewTestPath" }
                    });
                    Assert.AreEqual(200, updateTransactionTypeResult.Status);
                    Assert.IsTrue(updateTransactionTypeResult.Ok);
                    Assert.IsInstanceOf<TransactionTypeSimpleResponse>(updateTransactionTypeResult.Response);

                    // create transaction test                    
                    var createResult = await _dragonchainLevel1Client.CreateTransaction(transactionType, new { }, "pottery");
                    Assert.AreEqual(201, createResult.Status);
                    Assert.IsTrue(createResult.Ok);
                    Assert.IsInstanceOf<DragonchainTransactionCreateResponse>(createResult.Response);
                    Assert.IsNotEmpty(createResult.Response.TransactionId);

                    // create bulk transaction test
                    var newBulkTransaction = new BulkTransactionPayload
                    {
                        TransactionType = transactionType,                        
                        Tag = "pottery",
                        Payload = new { }
                    };
                    var createBulkResult = await _dragonchainLevel1Client.CreateBulkTransaction(                    
                        new List<BulkTransactionPayload> { newBulkTransaction, newBulkTransaction, newBulkTransaction }
                    );
                    Assert.AreEqual(207, createBulkResult.Status);
                    Assert.IsTrue(createBulkResult.Ok);
                    Assert.IsInstanceOf<DragonchainBulkTransactionCreateResponse>(createBulkResult.Response);
                }
                finally
                {
                    // delete transaction test
                    var deleteResult = await _dragonchainLevel1Client.DeleteTransactionType(transactionType);
                    Assert.AreEqual(200, deleteResult.Status);
                    Assert.IsTrue(deleteResult.Ok);
                    Assert.IsInstanceOf<TransactionTypeSimpleResponse>(deleteResult.Response);
                }
            }
            else
            {
                Assert.Warn("User secrets - dragonchain-sdk.tests-79a3edd0-2092-40a2-a04d-dcb46d5ca9ed not available");
            }
        }

        [Test]
        public async Task CreateContract_Test()
        {
            // use task delay to ensure contract is ready to be retrieved and deleted
            if (AreLevel1TestsConfigured())
            {
                var createContractResponse = await _dragonchainLevel1Client.CreateSmartContract("Test", "ubuntu:latest", "python3.6", new string[] { "something" },
                    SmartContractExecutionOrder.Serial, new { Test = "env" });
                Assert.AreEqual(202, createContractResponse.Status);
                Assert.IsTrue(createContractResponse.Ok);
                Assert.IsInstanceOf<DragonchainContractResponse>(createContractResponse.Response);
                await Task.Delay(30000);
                try
                {
                    var getContractResponse = await _dragonchainLevel1Client.GetSmartContract(createContractResponse.Response.Success.Id, null);
                    Assert.AreEqual(200, getContractResponse.Status);
                    Assert.IsTrue(getContractResponse.Ok);
                    Assert.IsInstanceOf<SmartContractAtRest>(getContractResponse.Response);
                    Assert.AreEqual(createContractResponse.Response.Success.Id, getContractResponse.Response.Id);
                    
                    var updateContractResponse = await _dragonchainLevel1Client.UpdateSmartContract(createContractResponse.Response.Success.Id, cmd: "python3.7");
                    Assert.AreEqual(202, updateContractResponse.Status);
                    Assert.IsTrue(updateContractResponse.Ok);
                    Assert.IsInstanceOf<DragonchainContractResponse>(updateContractResponse.Response);
                    Assert.AreEqual("updating", updateContractResponse.Response.Success.Status.State);
                    await Task.Delay(30000);
                }
                finally
                {
                    var deleteContractResponse = await _dragonchainLevel1Client.DeleteSmartContract(createContractResponse.Response.Success.Id);
                    Assert.AreEqual(202, deleteContractResponse.Status);
                    Assert.IsTrue(deleteContractResponse.Ok);
                    Assert.IsInstanceOf<SmartContractAtRest>(deleteContractResponse.Response);
                }
            }
            else
            {
                Assert.Warn("User secrets - dragonchain-sdk.tests-79a3edd0-2092-40a2-a04d-dcb46d5ca9ed not available");
            }
        }

        [Test]
        public async Task GetStatus_Test()
        {            
            if (AreLevel1TestsConfigured())
            {                
                var result = await _dragonchainLevel1Client.GetStatus();                
                Assert.AreEqual(200, result.Status);
                Assert.IsTrue(result.Ok);
                Assert.IsInstanceOf<L1DragonchainStatusResult>(result.Response);
            }
            else
            {
                Assert.Warn("User secrets - dragonchain-sdk.tests-79a3edd0-2092-40a2-a04d-dcb46d5ca9ed not available");
            }
        }

        [Test]
        public async Task QueryTransactionsAndGetTransaction_Test()
        {
            if (AreLevel1TestsConfigured())
            {
                // query transactions test
                var queryResult = await _dragonchainLevel1Client.QueryTransactions();
                Assert.AreEqual(200, queryResult.Status);
                Assert.IsTrue(queryResult.Ok);
                Assert.IsInstanceOf<QueryResult<L1DragonchainTransactionFull>>(queryResult.Response);

                //if (queryResult.Response.Results.Any())
                //{
                //    // get transaction test
                //    var result = await _dragonchainLevel1Client.GetTransaction(queryResult.Response.Results.First().Header.TransactionId);
                //    Assert.AreEqual(200, result.Status);
                //    Assert.IsTrue(result.Ok);
                //    Assert.IsInstanceOf<L1DragonchainTransactionFull>(result.Response);
                //}
            }
            else
            {
                Assert.Warn("User secrets - dragonchain-sdk.tests-79a3edd0-2092-40a2-a04d-dcb46d5ca9ed not available");
            }
        }

        [Test]
        public async Task QueryBlocksAndGetBlock_Test()
        {
            if (AreLevel1TestsConfigured())
            {
                // query blocks test
                var queryResult = await _dragonchainLevel1Client.QueryBlocks();
                Assert.AreEqual(200, queryResult.Status);
                Assert.IsTrue(queryResult.Ok);
                Assert.IsInstanceOf<QueryResult<BlockSchemaType>>(queryResult.Response);

                if (queryResult.Response.Results.Any())
                {
                    // get block test
                    var result = await _dragonchainLevel1Client.GetBlock(queryResult.Response.Results.First().Header.BlockId);
                    Assert.AreEqual(200, result.Status);
                    Assert.IsTrue(result.Ok);
                    Assert.IsInstanceOf<BlockSchemaType>(result.Response);
                }
            }
            else
            {
                Assert.Warn("User secrets - dragonchain-sdk.tests-79a3edd0-2092-40a2-a04d-dcb46d5ca9ed not available");
            }
        }

        [Test]
        public async Task QuerySmartContractsAndGetContract_Test()
        {
            if (AreLevel1TestsConfigured())
            {
                // query contracts test
                var queryResult = await _dragonchainLevel1Client.QuerySmartContracts();
                Assert.AreEqual(200, queryResult.Status);
                Assert.IsTrue(queryResult.Ok);
                Assert.IsInstanceOf<QueryResult<SmartContractAtRest>>(queryResult.Response);

                if (queryResult.Response.Results.Any())
                {
                    // get contract test
                    var result = await _dragonchainLevel1Client.GetSmartContract(queryResult.Response.Results.First().Id, null);
                    Assert.AreEqual(200, result.Status);
                    Assert.IsTrue(result.Ok);
                    Assert.IsInstanceOf<SmartContractAtRest>(result.Response);
                }
            }
            else
            {
                Assert.Warn("User secrets - dragonchain-sdk.tests-79a3edd0-2092-40a2-a04d-dcb46d5ca9ed not available");
            }
        }

        private int QueryResult<T>(QueryResult<T> response)
        {
            throw new NotImplementedException();
        }

        [Test]
        public async Task GetVerifications_Test()
        {
            if (AreLevel1TestsConfigured())
            {
                var queryResult = await _dragonchainLevel1Client.QueryBlocks();
                if (queryResult.Response.Results.Any())
                {
                    // get verifications test
                    var result = await _dragonchainLevel1Client.GetVerifications(queryResult.Response.Results.First().Header.BlockId);
                    Assert.AreEqual(200, result.Status);
                    Assert.IsTrue(result.Ok);
                    Assert.IsInstanceOf<Verifications>(result.Response);

                    // get verifications for level test
                    var levelResult = await _dragonchainLevel1Client.GetVerifications(queryResult.Response.Results.First().Header.BlockId, 2);
                    Assert.AreEqual(200, levelResult.Status);
                    Assert.IsTrue(levelResult.Ok);
                    Assert.IsInstanceOf<LevelVerifications>(levelResult.Response);
                }
            }
            else
            {
                Assert.Warn("User secrets - dragonchain-sdk.tests-79a3edd0-2092-40a2-a04d-dcb46d5ca9ed not available");
            }
        }

        [Test]
        public async Task ListTransactionTypes_Test()
        {
            if (AreLevel1TestsConfigured())
            {
                var result = await _dragonchainLevel1Client.ListTransactionTypes();
                Assert.AreEqual(200, result.Status);
                Assert.IsTrue(result.Ok);
                Assert.IsInstanceOf<TransactionTypeListResponse>(result.Response);
            }
            else
            {
                Assert.Warn("User secrets - dragonchain-sdk.tests-79a3edd0-2092-40a2-a04d-dcb46d5ca9ed not available");
            }
        }

        //[Test]
        //public async Task UpdateDragonnetConfig_Test()
        //{
        //    if (AreLevel1TestsConfigured())
        //    {
        //        var result = await _dragonchainLevel1Client.UpdateDragonnetConfig(new DragonnetConfigSchema { L2 = (decimal)0.123 });
        //        Assert.AreEqual(200, result.Status);
        //        Assert.IsTrue(result.Ok);
        //        Assert.IsInstanceOf<UpdateResponse>(result.Response);
        //    }
        //    else
        //    {
        //        Assert.Warn("User secrets - dragonchain-sdk.tests-79a3edd0-2092-40a2-a04d-dcb46d5ca9ed not available");
        //    }
        //}

        //[Test]
        //public async Task UpdateMatchmakingConfig_Test()
        //{
        //    if (AreLevel2TestsConfigured())
        //    {
        //        var result = await _dragonchainLevel2Client.UpdateMatchmakingConfig(askingPrice: (decimal)0.1);
        //        Assert.AreEqual(200, result.Status);
        //        Assert.IsTrue(result.Ok);
        //        Assert.IsInstanceOf<UpdateResponse>(result.Response);
        //    }
        //    else
        //    {
        //        Assert.Warn("User secrets - dragonchain-sdk.tests-79a3edd0-2092-40a2-a04d-dcb46d5ca9ed not available");
        //    }
        //}    

        public bool AreLevel1TestsConfigured()
        {
            return _dragonchainLevel1Client != null;            
        }

        public bool AreLevel2TestsConfigured()
        {
            return _dragonchainLevel2Client != null;
            
        }        

        private bool AreRequiredUserSecretsAvailable(string id, string authKey, string authKeyId)
        {
            return (!string.IsNullOrWhiteSpace(id) & !string.IsNullOrWhiteSpace(authKey) && !string.IsNullOrWhiteSpace(authKeyId));
        }
    }
}

/**
 * All Humans are welcome.
 */
