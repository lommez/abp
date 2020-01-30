using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.CosmosDB;
using Volo.Abp.MultiTenancy;

namespace Volo.Abp.Domain.Repositories.CosmosDB
{
    public abstract class CosmosDBRepositoryBase<TEntity, TPartitionKeyType> : BasicCosmosDBRepositoryBase<TEntity, TPartitionKeyType>, ICosmosDBRepository<TEntity, TPartitionKeyType>
        where TEntity : class, ICosmosDBEntity<TPartitionKeyType>
    {
        public IDataFilter DataFilter { get; set; }

        public ICurrentTenant CurrentTenant { get; set; }

        protected abstract IAsyncEnumerable<TEntity> GetAsyncEnumerable(
            Expression<Func<TEntity, bool>> expression = null,
            int? skip = null,
            int? take = null,
            Expression<Func<TEntity, object>> orderExpression = null,
            bool orderDescending = false,
            object partitionKeyValue = null,
            CancellationToken cancellationToken = default);

        public abstract Task DeleteAsync([NotNull] Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        protected virtual TQueryable ApplyDataFilters<TQueryable>(TQueryable query)
            where TQueryable : IQueryable<TEntity>
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                query = (TQueryable)query.WhereIf(DataFilter.IsEnabled<ISoftDelete>(), e => ((ISoftDelete)e).IsDeleted == false);
            }

            if (typeof(IMultiTenant).IsAssignableFrom(typeof(TEntity)))
            {
                var tenantId = CurrentTenant.Id;
                query = (TQueryable)query.WhereIf(DataFilter.IsEnabled<IMultiTenant>(), e => ((IMultiTenant)e).TenantId == tenantId);
            }

            return query;
        }

        protected virtual TQueryable ApplyConcurrencyStamp<TQueryable>(TQueryable query, TEntity entity, bool withConcurrencyStamp = false, string concurrencyStamp = null)
            where TQueryable : IQueryable<TEntity>
        {
            if (!withConcurrencyStamp || !(entity is IHasConcurrencyStamp entityWithConcurrencyStamp))
            {
                return query;
            }

            query = (TQueryable)query.Where(x => ((IHasConcurrencyStamp)x).ConcurrencyStamp == concurrencyStamp);

            return query;
        }
    }
}