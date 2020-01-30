using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.CosmosDB;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace Volo.Abp.Domain.Repositories.CosmosDB
{
    public abstract class BasicCosmosDBRepositoryBase<TEntity, TPartitionKeyType> :
        IBasicCosmosDBRepository<TEntity, TPartitionKeyType>,
        IServiceProviderAccessor,
        IUnitOfWorkEnabled,
        ITransientDependency
        where TEntity : class, ICosmosDBEntity<TPartitionKeyType>
    {
        public IServiceProvider ServiceProvider { get; set; }

        public ICancellationTokenProvider CancellationTokenProvider { get; set; }

        protected BasicCosmosDBRepositoryBase()
        {
            CancellationTokenProvider = NullCancellationTokenProvider.Instance;
        }

        public abstract Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

        public abstract Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        public abstract Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

        public async Task DeleteAsync(
            string id,
            object partitionKeyValue = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await GetAsync(id, partitionKeyValue, GetCancellationToken(cancellationToken)).ConfigureAwait(false);
            await DeleteAsync(entity, GetCancellationToken(cancellationToken)).ConfigureAwait(false);
        }

        public abstract Task<List<TEntity>> GetListAsync(
            Expression<Func<TEntity, bool>> expression = null,
            int? skip = null,
            int? take = null,
            Expression<Func<TEntity, object>> orderExpression = null,
            bool orderDescending = false,
            object partitionKeyValue = null,
            CancellationToken cancellationToken = default);

        public abstract Task<long> GetCountAsync(Expression<Func<TEntity, bool>> expression = null, object partitionKeyValue = null, CancellationToken cancellationToken = default);

        protected virtual CancellationToken GetCancellationToken(CancellationToken prefferedValue = default)
        {
            return CancellationTokenProvider.FallbackToProvider(prefferedValue);
        }

        public virtual async Task<TEntity> GetAsync(string id, object partitionKeyValue = null, CancellationToken cancellationToken = default)
        {
            var entityFound = await FindAsync(id, partitionKeyValue).ConfigureAwait(false);

            if (entityFound == null)
            {
                throw new EntityNotFoundException(typeof(TEntity), id);
            }

            return entityFound;
        }

        public abstract Task<TEntity> FindAsync(string id, object partitionKeyValue = null, CancellationToken cancellationToken = default);

        public abstract Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression = null, object partitionKeyValue = null, CancellationToken cancellationToken = default);

        protected virtual QueryRequestOptions EnsureRequestOptions(object partitionKeyValue)
        {
            var options = new QueryRequestOptions();
            if (partitionKeyValue != null)
            {
                options.PartitionKey = CreatePartitionKey(partitionKeyValue);
            }

            return options;
        }

        protected virtual PartitionKey CreatePartitionKey(TEntity entity)
        {
            if (entity.PartitionKeyValue.GetType() == typeof(string))
            {
                return new PartitionKey(entity.PartitionKeyValue as string);
            }
            else if (entity.PartitionKeyValue.GetType() == typeof(bool))
            {
                return new PartitionKey(Convert.ToBoolean(entity.PartitionKeyValue));
            }
            else if (entity.PartitionKeyValue.GetType() == typeof(double))
            {
                return new PartitionKey(Convert.ToDouble(entity.PartitionKeyValue));
            }
            else
                throw new AbpException("Invalid type for PartitionKey. The type valid are string, double and boolean.");
        }

        protected virtual PartitionKey CreatePartitionKey(object value)
        {
            if (value.GetType() == typeof(string))
            {
                return new PartitionKey(value as string);
            }
            else if (value.GetType() == typeof(bool))
            {
                return new PartitionKey(Convert.ToBoolean(value));
            }
            else if (value.GetType() == typeof(double))
            {
                return new PartitionKey(Convert.ToDouble(value));
            }
            else
                throw new AbpException("Invalid type for PartitionKey. The type valid are string, double and boolean.");
        }
    }
}