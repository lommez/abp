using JetBrains.Annotations;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abp.Domain.Repositories.CosmosDB
{
    public interface ICosmosDBRepository<TEntity, TPartitionKeyType> : IBasicCosmosDBRepository<TEntity, TPartitionKeyType>
        where TEntity : class, ICosmosDBEntity<TPartitionKeyType>
    {
        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task DeleteAsync([NotNull] Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    }
}