using Microsoft.Azure.Cosmos;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abp.CosmosDB
{
    public interface ICosmosDBCollection<TEntity>
        where TEntity : class, ICosmosDBEntity
    {
        Task<TEntity> ReadDocumentAsync(
            string entityId,
            PartitionKey partitionKey,
            ItemRequestOptions itemRequestOptions = null,
            CancellationToken cancellationToken = default);

        Task<TEntity> CreateDocumentAsync(
            TEntity entity,
            PartitionKey partitionKey,
            ItemRequestOptions itemRequestOptions = null,
            bool disableAutomaticIdGeneration = false,
            CancellationToken cancellationToken = default);

        Task<TEntity> ReplaceDocumentAsync(
            TEntity entity,
            string entityId,
            PartitionKey partitionKey,
            ItemRequestOptions itemRequestOptions = null,
            CancellationToken cancellationToken = default);

        Task<TEntity> DeleteDocumentAsync(
            string entityId,
            PartitionKey partitionKey,
            ItemRequestOptions itemRequestOptions = null,
            CancellationToken cancellationToken = default);

        IOrderedQueryable<TEntity> GetQueryable(QueryRequestOptions queryRequestOptions = null);
    }
}