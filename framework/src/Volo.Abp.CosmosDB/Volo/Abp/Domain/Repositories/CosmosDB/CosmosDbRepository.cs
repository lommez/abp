using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.CosmosDB;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.CosmosDB;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Guids;
using Volo.Abp.Threading;

namespace Volo.Abp.Domain.Repositories.CosmosDB
{
    public class CosmosDBRepository<TCosmosDBContext, TEntity> : CosmosDBRepositoryBase<TEntity>, ICosmosDBRepository<TEntity>
        where TCosmosDBContext : IAbpCosmosDBContext
        where TEntity : class, ICosmosDBEntity
    {
        protected string DatabaseName { get; set; }

        protected string CollectionName { get; set; }

        public virtual ICosmosDBCollection<TEntity> Collection => DbContext.Collection<TEntity>();

        public virtual TCosmosDBContext DbContext => DbContextProvider.GetDbContext();

        protected ICosmosDBContextProvider<TCosmosDBContext> DbContextProvider { get; }

        public ILocalEventBus LocalEventBus { get; set; }

        public IDistributedEventBus DistributedEventBus { get; set; }

        public IEntityChangeEventHelper EntityChangeEventHelper { get; set; }

        public IGuidGenerator GuidGenerator { get; set; }

        public IAuditPropertySetter AuditPropertySetter { get; set; }

        public PartitionKey PartitionKey { get; set; }

        public CosmosDBRepository(ICosmosDBContextProvider<TCosmosDBContext> dbContextProvider)
        {
            DbContextProvider = dbContextProvider;

            LocalEventBus = NullLocalEventBus.Instance;
            DistributedEventBus = NullDistributedEventBus.Instance;
            EntityChangeEventHelper = NullEntityChangeEventHelper.Instance;
        }

        public override void Delete(TEntity entity)
        {
            AsyncHelper.RunSync(() => ApplyAbpConceptsForDeletedEntityAsync(entity));

            if (entity is ISoftDelete softDeleteEntity)
            {
                throw new NotImplementedException("Soft delete not implemented");
            }
            else
            {
                Collection.DeleteDocumentAsync(entity.Id, PartitionKey).GetAwaiter().GetResult();
            }
        }

        public override void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = GetQueryable()
                .Where(predicate)
                .ToList();

            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }

        public override async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await ApplyAbpConceptsForDeletedEntityAsync(entity);

            if (entity is ISoftDelete softDeleteEntity)
            {
                throw new NotImplementedException("Soft delete not implemented");
            }
            else
            {
                await Collection.DeleteDocumentAsync(entity.Id, PartitionKey, null, cancellationToken);
            }
        }

        public override TEntity Find(string id)
        {
            TEntity document = null;
            try
            {
                // Read the item to see if it exists.
                document = Collection.ReadDocumentAsync(id, PartitionKey).GetAwaiter().GetResult();
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
            }
            return document;
        }

        public override long GetCount()
        {
            return GetList().Count;
        }

        public override async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        {
            var result = await GetListAsync();
            return result.Count;
        }

        public override List<TEntity> GetList()
        {
            return GetQueryable().ToList();
        }

        public override Task<List<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(GetList());
        }

        protected override IQueryable<TEntity> GetQueryable()
        {
            var query = Collection.GetQueryable();
            return query;
        }

        public override TEntity Insert(TEntity entity)
        {
            AsyncHelper.RunSync(() => ApplyAbpConceptsForAddedEntityAsync(entity));

            var document = Collection.CreateDocumentAsync(entity, PartitionKey).GetAwaiter().GetResult();
            return document;
        }

        public override async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await ApplyAbpConceptsForAddedEntityAsync(entity);

            var document = await Collection.CreateDocumentAsync(entity, PartitionKey);
            return document;
        }

        public override TEntity Update(TEntity entity)
        {
            SetModificationAuditProperties(entity);

            if (entity is ISoftDelete softDeleteEntity && softDeleteEntity.IsDeleted)
            {
                throw new NotImplementedException("Soft delete not implemented");
                //SetDeletionAuditProperties(entity);
                //AsyncHelper.RunSync(() => TriggerEntityDeleteEventsAsync(entity));
            }
            else
            {
                AsyncHelper.RunSync(() => TriggerEntityUpdateEventsAsync(entity));
            }

            AsyncHelper.RunSync(() => TriggerDomainEventsAsync(entity));

            var document = Collection.ReplaceDocumentAsync(entity, entity.Id, PartitionKey).GetAwaiter().GetResult();
            return document;
        }

        public override async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            SetModificationAuditProperties(entity);

            if (entity is ISoftDelete softDeleteEntity && softDeleteEntity.IsDeleted)
            {
                throw new NotImplementedException("Soft delete not implemented");
                //SetDeletionAuditProperties(entity);
                //await TriggerEntityDeleteEventsAsync(entity);
            }
            else
            {
                await TriggerEntityUpdateEventsAsync(entity);
            }

            await TriggerDomainEventsAsync(entity);

            var document = Collection.ReplaceDocumentAsync(entity, entity.Id, PartitionKey, cancellationToken: GetCancellationToken(cancellationToken));

            return (TEntity)(dynamic)document;
        }

        protected virtual async Task ApplyAbpConceptsForAddedEntityAsync(TEntity entity)
        {
            CheckAndSetId(entity);
            SetCreationAuditProperties(entity);
            await TriggerEntityCreateEvents(entity);
            await TriggerDomainEventsAsync(entity);
        }

        private async Task TriggerEntityCreateEvents(TEntity entity)
        {
            await EntityChangeEventHelper.TriggerEntityCreatedEventOnUowCompletedAsync(entity);
            await EntityChangeEventHelper.TriggerEntityCreatingEventAsync(entity);
        }

        protected virtual async Task TriggerEntityUpdateEventsAsync(TEntity entity)
        {
            await EntityChangeEventHelper.TriggerEntityUpdatedEventOnUowCompletedAsync(entity);
            await EntityChangeEventHelper.TriggerEntityUpdatingEventAsync(entity);
        }

        protected virtual async Task ApplyAbpConceptsForDeletedEntityAsync(TEntity entity)
        {
            SetDeletionAuditProperties(entity);
            await TriggerEntityDeleteEventsAsync(entity);
            await TriggerDomainEventsAsync(entity);
        }

        protected virtual async Task TriggerEntityDeleteEventsAsync(TEntity entity)
        {
            await EntityChangeEventHelper.TriggerEntityDeletedEventOnUowCompletedAsync(entity);
            await EntityChangeEventHelper.TriggerEntityDeletingEventAsync(entity);
        }

        protected virtual void CheckAndSetId(TEntity entity)
        {
            if (entity is ICosmosDBEntity entityWithGuidId)
            {
                TrySetGuidId(entityWithGuidId);
            }
        }

        protected virtual void TrySetGuidId(ICosmosDBEntity entity)
        {
            if (entity.Id != default)
            {
                return;
            }

            EntityHelper.TrySetId(
                entity,
                () => GuidGenerator.Create().ToString(),
                true
            );
        }

        protected virtual void SetCreationAuditProperties(TEntity entity)
        {
            AuditPropertySetter.SetCreationProperties(entity);
        }

        protected virtual void SetModificationAuditProperties(TEntity entity)
        {
            AuditPropertySetter.SetModificationProperties(entity);
        }

        protected virtual void SetDeletionAuditProperties(TEntity entity)
        {
            AuditPropertySetter.SetDeletionProperties(entity);
        }

        protected virtual async Task TriggerDomainEventsAsync(object entity)
        {
            var generatesDomainEventsEntity = entity as IGeneratesDomainEvents;
            if (generatesDomainEventsEntity == null)
            {
                return;
            }

            var localEvents = generatesDomainEventsEntity.GetLocalEvents().ToArray();
            if (localEvents.Any())
            {
                foreach (var localEvent in localEvents)
                {
                    await LocalEventBus.PublishAsync(localEvent.GetType(), localEvent);
                }

                generatesDomainEventsEntity.ClearLocalEvents();
            }

            var distributedEvents = generatesDomainEventsEntity.GetDistributedEvents().ToArray();
            if (distributedEvents.Any())
            {
                foreach (var distributedEvent in distributedEvents)
                {
                    await DistributedEventBus.PublishAsync(distributedEvent.GetType(), distributedEvent);
                }

                generatesDomainEventsEntity.ClearDistributedEvents();
            }
        }

        /// <summary>
        /// Sets a new <see cref="IHasConcurrencyStamp.ConcurrencyStamp"/> value
        /// if given entity implements <see cref="IHasConcurrencyStamp"/> interface.
        /// Returns the old <see cref="IHasConcurrencyStamp.ConcurrencyStamp"/> value.
        /// </summary>
        protected virtual string SetNewConcurrencyStamp(TEntity entity)
        {
            if (!(entity is IHasConcurrencyStamp concurrencyStampEntity))
            {
                return null;
            }

            var oldConcurrencyStamp = concurrencyStampEntity.ConcurrencyStamp;
            concurrencyStampEntity.ConcurrencyStamp = Guid.NewGuid().ToString("N");
            return oldConcurrencyStamp;
        }

        protected virtual void ThrowOptimisticConcurrencyException()
        {
            throw new AbpDbConcurrencyException("Database operation expected to affect 1 row but actually affected 0 row. Data may have been modified or deleted since entities were loaded. This exception has been thrown on optimistic concurrency check.");
        }
    }
}