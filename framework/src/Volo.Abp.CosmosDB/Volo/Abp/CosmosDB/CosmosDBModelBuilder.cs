using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abp.CosmosDB
{
    public class CosmosDBModelBuilder : ICosmosDBModelBuilder
    {
        private readonly Dictionary<Type, object> _entityModelBuilders;

        private static readonly object SyncObj = new object();

        public CosmosDBModelBuilder()
        {
            _entityModelBuilders = new Dictionary<Type, object>();
        }

        public CosmosDBContextModel Build()
        {
            lock (SyncObj)
            {
                var entityModels = _entityModelBuilders
                    .Select(x => x.Value)
                    .Cast<ICosmosDBEntityModel>()
                    .ToImmutableDictionary(x => x.EntityType, x => x);

                return new CosmosDBContextModel(entityModels);
            }
        }

        public virtual void Entity<TEntity>(Action<ICosmosDBEntityModelBuilder<TEntity>> buildAction = null)
        {
            var model = (ICosmosDBEntityModelBuilder<TEntity>)_entityModelBuilders.GetOrAdd(
                typeof(TEntity),
                () => new CosmosDBEntityModelBuilder<TEntity>()
            );

            buildAction?.Invoke(model);
        }

        public virtual void Entity(Type entityType, Action<ICosmosDBEntityModelBuilder> buildAction = null)
        {
            Check.NotNull(entityType, nameof(entityType));

            var model = (ICosmosDBEntityModelBuilder)_entityModelBuilders.GetOrAdd(
                entityType,
                () => (ICosmosDBEntityModelBuilder)Activator.CreateInstance(
                    typeof(CosmosDBEntityModelBuilder<>).MakeGenericType(entityType)
                )
            );

            buildAction?.Invoke(model);
        }

        public virtual IReadOnlyList<ICosmosDBEntityModel> GetEntities()
        {
            return _entityModelBuilders.Values.Cast<ICosmosDBEntityModel>().ToImmutableList();
        }
    }
}