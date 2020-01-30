using JetBrains.Annotations;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abp.Domain.Repositories.CosmosDB
{
    public interface IBasicCosmosDBRepository<TEntity, TPartitionKeyType> : IReadOnlyBasicCosmosDBRepository<TEntity, TPartitionKeyType>
        where TEntity : class, ICosmosDBEntity<TPartitionKeyType>
    {
        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <param name="entity">Inserted entity</param>
        [NotNull]
        Task<TEntity> InsertAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <param name="entity">Entity</param>
        [NotNull]
        Task<TEntity> UpdateAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task DeleteAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        /// <param name="partitionKeyValue">Value of the partition key</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task DeleteAsync(string id, object partitionKeyValue = null, CancellationToken cancellationToken = default);  //TODO: Return true if deleted
    }
}