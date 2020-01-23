using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities.CosmosDB;
using Volo.Abp.MultiTenancy;

namespace Volo.Abp.Domain.Repositories.CosmosDB
{
    public abstract class CosmosDBRepositoryBase<TEntity, TPartitionKeyType> : BasicCosmosDBRepositoryBase<TEntity, TPartitionKeyType>, ICosmosDBRepository<TEntity, TPartitionKeyType>
        where TEntity : class, ICosmosDBEntity<TPartitionKeyType>
    {
        public IDataFilter DataFilter { get; set; }

        public ICurrentTenant CurrentTenant { get; set; }

        public virtual Type ElementType => GetQueryable().ElementType;

        public virtual Expression Expression => GetQueryable().Expression;

        public virtual IQueryProvider Provider => GetQueryable().Provider;

        public virtual IQueryable<TEntity> WithDetails()
        {
            return GetQueryable();
        }

        public virtual IQueryable<TEntity> WithDetails(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            return GetQueryable();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return GetQueryable().GetEnumerator();
        }

        protected abstract IQueryable<TEntity> GetQueryable();

        protected abstract Task<IEnumerable<TEntity>> GetEnumerableAsync(
            Expression<Func<TEntity, bool>> expression = null,
            int? skip = null,
            int? take = null,
            Expression<Func<TEntity, object>> orderExpression = null,
            bool orderDescending = false,
            object partitionKeyValue = null, 
            CancellationToken cancellationToken = default);

        protected abstract IAsyncEnumerable<TEntity> GetEnumerable(
            Expression<Func<TEntity, bool>> expression = null,
            int? skip = null,
            int? take = null,
            Expression<Func<TEntity, object>> orderExpression = null,
            bool orderDescending = false,
            object partitionKeyValue = null,
            CancellationToken cancellationToken = default);

        public abstract Task DeleteAsync([NotNull] Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        public override async Task DeleteAsync(string id, object partitionKeyValue, CancellationToken cancellationToken = default)
        {
            var entity = await FindAsync(id, partitionKeyValue, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (entity == null)
            {
                return;
            }

            await DeleteAsync(entity, cancellationToken).ConfigureAwait(false);
        }

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
    }
}