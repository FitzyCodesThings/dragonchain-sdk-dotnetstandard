using System.Collections.Generic;
using System.Threading.Tasks;
using DragonchainSDK.Blocks;
using DragonchainSDK.Contracts;
using DragonchainSDK.Credentials.Keys;
using DragonchainSDK.Framework.Web;
using DragonchainSDK.Interchain;
using DragonchainSDK.Interchain.Bitcoin;
using DragonchainSDK.Interchain.Networks;
using DragonchainSDK.Status;
using DragonchainSDK.Transactions;
using DragonchainSDK.Transactions.Bulk;
using DragonchainSDK.Transactions.L1;
using DragonchainSDK.Transactions.Types;

namespace DragonchainSDK
{
    public interface IDragonchainClient
    {
        Task<ApiResponse<CreateAPIKeyResponse>> CreateApiKey();
        Task<ApiResponse<PublicBlockchainTransactionResponse>> CreateBitcoinTransaction(BitcoinNetwork? network, decimal? satoshisPerByte, string data, string changeAddress, IEnumerable<BitcoinTransactionOutputs> outputs);
        Task<ApiResponse<DragonchainBulkTransactionCreateResponse>> CreateBulkTransaction(IEnumerable<BulkTransactionPayload> transactionList);
        Task<ApiResponse<PublicBlockchainTransactionResponse>> CreateEthereumTransaction(EthereumNetwork? network, string to, string value, string data, string gasPrice, string gas);
        Task<ApiResponse<DragonchainContractResponse>> CreateSmartContract(string transactionType, string image, string cmd, IEnumerable<string> args, SmartContractExecutionOrder executionOrder = SmartContractExecutionOrder.Parallel, object environmentVariables = null, object secrets = null, int? scheduleIntervalInSeconds = null, string cronExpression = null, string registryCredentials = null);
        Task<ApiResponse<DragonchainTransactionCreateResponse>> CreateTransaction(string transactionType, object payload = null, string tag = "", string callbackURL = "");
        Task<ApiResponse<TransactionTypeSimpleResponse>> CreateTransactionType(string transactionType, IEnumerable<TransactionTypeCustomIndex> customIndexes);
        Task<ApiResponse<DeleteAPIKeyResponse>> DeleteApiKey(string keyId);
        Task<ApiResponse<SmartContractAtRest>> DeleteSmartContract(string smartContractId);
        Task<ApiResponse<TransactionTypeSimpleResponse>> DeleteTransactionType(string transactionType);
        Task<ApiResponse<GetAPIKeyResponse>> GetApiKey(string keyId);
        Task<ApiResponse<BlockSchemaType>> GetBlock(string blockId);
        Task<ApiResponse<PublicBlockchainAddressListResponse>> GetPublicBlockchainAddresses();
        Task<ApiResponse<SmartContractAtRest>> GetSmartContract(string smartContractId, string transactionType);
        Task<ApiResponse<string>> GetSmartContractObject(string key, string smartContractId);
        Task<string> GetSmartContractSecret(string secretName);
        Task<ApiResponse<L1DragonchainStatusResult>> GetStatus();
        Task<ApiResponse<L1DragonchainTransactionFull>> GetTransaction(string transactionId);
        Task<ApiResponse<TransactionTypeResponse>> GetTransactionType(string transactionType);
        Task<ApiResponse<IVerifications>> GetVerifications(string blockId, int level = 0);
        Task<ApiResponse<ListAPIKeyResponse>> ListApiKeys();
        Task<ApiResponse<IEnumerable<string>>> ListSmartcontractHeap(string prefixKey, string smartContractId);
        Task<ApiResponse<TransactionTypeListResponse>> ListTransactionTypes();
        Task<ApiResponse<QueryResult<BlockSchemaType>>> QueryBlocks(string luceneQuery = "", string sort = "", int offset = 0, int limit = 10);
        Task<ApiResponse<QueryResult<SmartContractAtRest>>> QuerySmartContracts(string luceneQuery = "", string sort = "", int offset = 0, int limit = 10);
        Task<ApiResponse<QueryResult<L1DragonchainTransactionFull>>> QueryTransactions(string transactionType, string redisearchQuery = "*", bool verbatim = true, int offset = 0, int limit = 10, string sortBy = "", bool sortAscending = true, bool idsOnly = false);
        Task<ApiResponse<DragonchainContractResponse>> UpdateSmartContract(string smartContractId, string image = null, string cmd = null, string[] args = null, SmartContractExecutionOrder? executionOrder = null, bool? enabled = null, object environmentVariables = null, object secrets = null, int? scheduleIntervalInSeconds = null, string cronExpression = null, string registryCredentials = null);
        Task<ApiResponse<TransactionTypeSimpleResponse>> UpdateTransactionType(string transactionType, IEnumerable<TransactionTypeCustomIndex> customIndexes);
    }
}