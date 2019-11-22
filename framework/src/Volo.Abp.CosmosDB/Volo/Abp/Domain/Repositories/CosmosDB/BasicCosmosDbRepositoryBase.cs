using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.CosmosDB;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace Volo.Abp.Domain.Repositories.CosmosDB
{
    public abstract class BasicCosmosDbRepositoryBase<TEntity> :
        IBasicCosmosDbRepository<TEntity>,
        IServiceProviderAccessor,
        IUnitOfWorkEnabled,
        ITransientDependency
        where TEntity : class, ICosmosDbEntity
    {
        public IServiceProvider ServiceProvider { get; set; }

        public ICancellationTokenProvider CancellationTokenProvider { get; set; }

        protected BasicCosmosDbRepositoryBase()
        {
            CancellationTokenProvider = NullCancellationTokenProvider.Instance;
        }

        public abstract TEntity Insert(TEntity entity);

        public virtual Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Insert(entity));
        }

        public abstract TEntity Update(TEntity entity);

        public virtual Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Update(entity));
        }

        public abstract void Delete(TEntity entity);

        public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            Delete(entity);
            return Task.CompletedTask;
        }

        protected virtual CancellationToken GetCancellationToken(CancellationToken prefferedValue = default)
        {
            return CancellationTokenProvider.FallbackToProvider(prefferedValue);
        }

        public abstract List<TEntity> GetList();

        public virtual Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(GetList());
        }

        public abstract long GetCount();

        public virtual Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(GetCount());
        }

        public virtual TEntity Get(TEntity entity)
        {
            var entityFound = Find(entity);

            if (entityFound == null)
            {
                throw new EntityNotFoundException(typeof(TEntity), entity);
            }

            return entityFound;
        }

        public virtual Task<TEntity> GetAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Get(entity));
        }

        public abstract TEntity Find(TEntity entity);

        public virtual Task<TEntity> FindAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Find(entity));
        }
    }
}