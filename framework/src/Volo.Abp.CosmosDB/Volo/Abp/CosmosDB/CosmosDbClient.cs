//using Microsoft.Azure.Cosmos;
//using System;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Volo.Abp.CosmosDB
//{
//    public class CosmosDBClient : ICosmosDBClient
//    {
//        private readonly string _databaseName;
//        private readonly string _containerName;
//        private readonly CosmosClient _cosmosClient;
//        private readonly Container _container;

//        public CosmosDBClient(string databaseName, string containerName, CosmosClient cosmosClient)
//        {
//            _databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
//            _containerName = containerName ?? throw new ArgumentNullException(nameof(containerName));
//            _cosmosClient = cosmosClient ?? throw new ArgumentNullException(nameof(cosmosClient));
//            _container = _cosmosClient.GetContainer(_databaseName, _containerName);
//        }

//        public async Task<Document> ReadDocumentAsync<Document>(string documentId, PartitionKey partitionKey, RequestOptions options = null,
//            CancellationToken cancellationToken = default(CancellationToken))
//        {
//            _container.
//            return await _container.ReadItemAsync<Document>(documentId, options, cancellationToken);

//            return await _documentClient.ReadDocumentAsync(
//                UriFactory.CreateDocumentUri(_databaseName, _collectionName, documentId), options, cancellationToken);
//        }

//        public async Task<Document> CreateDocumentAsync(object document, RequestOptions options = null,
//            bool disableAutomaticIdGeneration = false, CancellationToken cancellationToken = default(CancellationToken))
//        {
//            return await _documentClient.CreateDocumentAsync(
//                UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), document, options,
//                disableAutomaticIdGeneration, cancellationToken);
//        }

//        public async Task<Document> ReplaceDocumentAsync(string documentId, object document,
//            RequestOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
//        {            
//            var result = await _documentClient.ReplaceDocumentAsync(
//                UriFactory.CreateDocumentUri(_databaseName, _collectionName, documentId), document, options,
//                cancellationToken);

//            return result;
//        }

//        public async Task<Document> DeleteDocumentAsync(string documentId, RequestOptions options = null,
//            CancellationToken cancellationToken = default(CancellationToken))
//        {
//            return await _documentClient.DeleteDocumentAsync(
//                UriFactory.CreateDocumentUri(_databaseName, _collectionName, documentId), options, cancellationToken);
//        }
//    }
//}