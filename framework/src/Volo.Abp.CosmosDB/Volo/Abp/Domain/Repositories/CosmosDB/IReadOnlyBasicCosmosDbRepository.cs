using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abp.Domain.Repositories.CosmosDB
{
    public interface IReadOnlyBasicCosmosDBRepository<TEntity> : IRepository
        where TEntity : class, ICosmosDBEntity
    {
        /// <summary>
        /// Gets a list of all the entities.
        /// </summary>
        /// <returns>Entity</returns>
        List<TEntity> GetList();

        /// <summary>
        /// Gets a list of all the entities.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Entity</returns>
        Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets total count of all entities.
        /// </summary>
        long GetCount();

        /// <summary>
        /// Gets total count of all entities.
        /// </summary>
        Task<long> GetCountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an entity with given primary key.
        /// Throws <see cref="EntityNotFoundException"/> if can not find an entity with given id.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        [NotNull]
        TEntity Get(string id);

        /// <summary>
        /// Gets an entity with given primary key.
        /// Throws <see cref="EntityNotFoundException"/> if can not find an entity with given id.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Entity</returns>
        [NotNull]
        Task<TEntity> GetAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an entity with given primary key or null if not found.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity or null</returns>
        [CanBeNull]
        TEntity Find(string id);

        /// <summary>
        /// Gets an entity with given primary key or null if not found.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Entity or null</returns>
        Task<TEntity> FindAsync(string id, CancellationToken cancellationToken = default);
    }
}