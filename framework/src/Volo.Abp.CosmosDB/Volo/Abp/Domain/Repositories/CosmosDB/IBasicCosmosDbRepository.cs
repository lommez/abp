using JetBrains.Annotations;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abp.Domain.Repositories.CosmosDB
{
    public interface IBasicCosmosDbRepository<TEntity> : IReadOnlyBasicCosmosDbRepository<TEntity>
        where TEntity : class, ICosmosDbEntity
    {
        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity</param>
        [NotNull]
        TEntity Insert([NotNull] TEntity entity);

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
        /// <param name="entity">Entity</param>
        [NotNull]
        TEntity Update([NotNull] TEntity entity);

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
        void Delete([NotNull] TEntity entity);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task DeleteAsync([NotNull] TEntity entity, CancellationToken cancellationToken = default);
    }
}