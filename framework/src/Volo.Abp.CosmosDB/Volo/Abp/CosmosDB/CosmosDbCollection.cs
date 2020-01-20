using Microsoft.Azure.Cosmos;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abp.CosmosDB
{
    public class CosmosDBCollection<TEntity, TPartitionKeyType> : ICosmosDBCollection<TEntity, TPartitionKeyType>
        where TEntity : class, ICosmosDBEntity<TPartitionKeyType>
    {
        private readonly Database _database;
        private readonly Container _container;
        private readonly string _collectionName;

        public CosmosDBCollection(Database database, string collectionName)
        {
            _database = database;
            _container = _database.GetContainer(collectionName);
            _collectionName = collectionName;
        }

        public async Task<TEntity> ReadDocumentAsync(
            string entityId,
            PartitionKey partitionKey,
            ItemRequestOptions itemRequestOptions = null,
            CancellationToken cancellationToken = default)
        {
            var document = await _container.ReadItemAsync<TEntity>(entityId, partitionKey, itemRequestOptions, cancellationToken);
            return document.Resource;
        }

        public async Task<TEntity> CreateDocumentAsync(
            TEntity entity,
            PartitionKey partitionKey,
            ItemRequestOptions itemRequestOptions = null,
            bool disableAutomaticIdGeneration = false,
            CancellationToken cancellationToken = default)
        {
            var document = await _container.CreateItemAsync(entity, partitionKey, itemRequestOptions, cancellationToken);
            return document.Resource;
        }

        public async Task<TEntity> ReplaceDocumentAsync(
            TEntity entity,
            string entityId,
            PartitionKey partitionKey,
            ItemRequestOptions itemRequestOptions = null,
            CancellationToken cancellationToken = default)
        {
            var document = await _container.ReplaceItemAsync(entity, entityId, partitionKey, itemRequestOptions, cancellationToken);
            return document.Resource;
        }

        public async Task<TEntity> DeleteDocumentAsync(
            string entityId,
            PartitionKey partitionKey,
            ItemRequestOptions itemRequestOptions = null,
            CancellationToken cancellationToken = default)
        {
            var document = await _container.DeleteItemAsync<TEntity>(entityId, partitionKey, itemRequestOptions, cancellationToken);
            return document.Resource;
        }

        public IOrderedQueryable<TEntity> GetQueryable(QueryRequestOptions queryRequestOptions = null)
        {
            return _container.GetItemLinqQueryable<TEntity>(requestOptions: queryRequestOptions);
        }
    }
}