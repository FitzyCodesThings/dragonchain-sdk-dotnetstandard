﻿using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using dragonchain_sdk.Contracts;
using dragonchain_sdk.Framework.Web;
using dragonchain_sdk.Status;
using dragonchain_sdk.Transactions.L1;
using dragonchain_sdk.Contracts.SmartContractAtRest;
using dragonchain_sdk.Blocks;
using dragonchain_sdk.Transactions;
using dragonchain_sdk.Shared;

namespace dragonchain_sdk.tests
{
    [TestFixture]
    public class DragonchainClientUnitTests
    {
        private Mock<IHttpService> _httpService;
        private ILogger _logger;
        private DragonchainClient _dragonchainClient;

        public DragonchainClientUnitTests()
        {   
            _logger = new NUnitLogger<DragonchainClientUnitTests>();
            _httpService = new Mock<IHttpService>();
            _dragonchainClient = new DragonchainClient("fakeDragonchainId", _logger, null, null, _httpService.Object);
        }

        [SetUp]
        public void BeforEach()
        {
            _httpService.Invocations.Clear();
        }
        
        [Test]
        public async Task GetStatus_CallswithCorrectParams_Test()
        {            
            await _dragonchainClient.GetStatus();
            _httpService.Verify(service => service.GetAsync<L1DragonchainStatusResult>("https://fakeDragonchainId.api.dragonchain.com/status"), Times.Once);
        }

        [Test]
        public async Task GetTransaction_CallswithCorrectParams_Test()
        {
            var id = "batman-transaction-id";
            await _dragonchainClient.GetTransaction(id);
            _httpService.Verify(service => service.GetAsync<L1DragonchainTransactionFull>($"https://fakeDragonchainId.api.dragonchain.com/transaction/{id}"), Times.Once);
        }

        [Test]
        public async Task SetDragonchainId_AllowsResettingtheDragonchainId_Test()
        {
            var dragonchainClient = new DragonchainClient("fakeDragonchainId", _logger, null, null, _httpService.Object);
            dragonchainClient.SetDragonchainId("hotBanana");
            await dragonchainClient.GetStatus();
            _httpService.Verify(service => service.GetAsync<L1DragonchainStatusResult>("https://hotBanana.api.dragonchain.com/status"), Times.Once);            
        }

        [Test]
        public async Task SetEndpoint_AllowsSettingtheEnpointManually_Test()
        {
            var dragonchainClient = new DragonchainClient("fakeDragonchainId", _logger, null, null, _httpService.Object);
            var endpoint = "https://some.domain.com";
            dragonchainClient.SetEndpoint(endpoint);
            await dragonchainClient.GetStatus();
            _httpService.Verify(service => service.GetAsync<L1DragonchainStatusResult>($"{endpoint}/status"), Times.Once);
        }

        [Test]
        public async Task GetBlock_CallswithCorrectParams_Test()
        {               
            var id = "robin-block-id";
            await _dragonchainClient.GetBlock(id);
            _httpService.Verify(service => service.GetAsync<L1DragonchainTransactionFull>($"https://fakeDragonchainId.api.dragonchain.com/block/{id}"), Times.Once);
        }

        [Test]
        public async Task GetSmartContract_CallswithCorrectParams_Test()
        {
            var id = "joker-smartcontract-id";
            await _dragonchainClient.GetSmartContract(id);
            _httpService.Verify(service => service.GetAsync<SmartContractAtRest>($"https://fakeDragonchainId.api.dragonchain.com/contract/{id}"), Times.Once);
        }

        [Test]
        public async Task GetVerification_CallswithCorrectParams_Test()
        {
            var levelVerificationsReponse = new ApiResponse<Verifications> { Ok = true, Status = 200, Response = new Verifications() };
            _httpService.Setup(service => service.GetAsync<Verifications>(It.IsAny<string>())).ReturnsAsync(levelVerificationsReponse);
            var id = "block_id";
            await _dragonchainClient.GetVerifications(id);            
            _httpService.Verify(service => service.GetAsync<Verifications>($"https://fakeDragonchainId.api.dragonchain.com/verifications/{id}"), Times.Once);

            var verificationsReponse = new ApiResponse<LevelVerifications> { Ok = true, Status = 200, Response = new LevelVerifications() };
            _httpService.Setup(service => service.GetAsync<LevelVerifications>(It.IsAny<string>())).ReturnsAsync(verificationsReponse);
            var level = 2;            
            await _dragonchainClient.GetVerifications(id, level);
            _httpService.Verify(service => service.GetAsync<LevelVerifications>($"https://fakeDragonchainId.api.dragonchain.com/verifications/{id}?level={level}"), Times.Once);
        }

        [Test]
        public async Task QueryBlocks_CallswithCorrectParams_Test()
        {
            var @params = "banana";
            await _dragonchainClient.QueryBlocks(@params);
            _httpService.Verify(service => service.GetAsync<DragonchainBlockQueryResult>($"https://fakeDragonchainId.api.dragonchain.com/block?q={@params}&offset=0&limit=10"), Times.Once);
        }

        [Test]
        public async Task QuerySmartContracts_CallswithCorrectParams_Test()
        {
            var @params = "banana";
            await _dragonchainClient.QuerySmartContracts(@params);
            _httpService.Verify(service => service.GetAsync<SmartContractAtRest>($"https://fakeDragonchainId.api.dragonchain.com/contract?q={@params}&offset=0&limit=10"), Times.Once);
        }

        [Test]
        public async Task DeleteSmartContract_CallswithCorrectParams_Test()
        {
            var id = "banana";
            await _dragonchainClient.DeleteSmartContract(id);
            _httpService.Verify(service => service.DeleteAsync<UpdateResponse>($"https://fakeDragonchainId.api.dragonchain.com/contract/{id}"), Times.Once);
        }

        [Test]
        public async Task CreateTransaction_CallswithCorrectParams_Test()
        {
            var transactionCreatePayload = new DragonchainTransactionCreatePayload
            {
                Version = "1",
                TxnType= "transaction",
                Payload = "hi!" ,
                Tag = "Awesome!"
            };
            await _dragonchainClient.CreateTransaction(transactionCreatePayload);
            _httpService.Verify(service => service.PostAsync<DragonchainTransactionCreateResponse>($"https://fakeDragonchainId.api.dragonchain.com/transaction", transactionCreatePayload), Times.Once);
        }

        [Test]
        public async Task CreateContract_CallswithCorrectParams_Test()
        {
            var contractPayload = new ContractCreationSchema
            {
                TxnType = "name",
                Image = "ubuntu:latest",
                ExecutionOrder = SmartContractExecutionOrder.Serial,
                Env = new { Banana = "banana", Apple = "banana" },
                Cmd = "banana",
                Args = new string[] { "-m cool" }
            };
            await _dragonchainClient.CreateContract(contractPayload);
            _httpService.Verify(service => service.PostAsync<DragonchainContractCreateResponse>($"https://fakeDragonchainId.api.dragonchain.com/contract", contractPayload), Times.Once);
        }

        [Test]
        public async Task UpdateSmartContract_CallswithCorrectParams_Test()
        {
            var id = "616152367378";
            var status = SmartContractDesiredState.Active;        
            
            await _dragonchainClient.UpdateSmartContract(id, null, null, null, status);
            _httpService.Verify(service => service.PutAsync<UpdateResponse>($"https://fakeDragonchainId.api.dragonchain.com/contract/{id}", It.IsAny<object>()), Times.Once);
        }
    }
}

/**
 * All Humans are welcome.
 */
