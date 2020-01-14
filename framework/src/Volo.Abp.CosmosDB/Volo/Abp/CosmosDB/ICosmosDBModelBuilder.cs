using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abp.CosmosDB
{
    public interface ICosmosDBModelBuilder
    {
        void Entity<TEntity>(Action<ICosmosDBEntityModelBuilder<TEntity>> buildAction = null);

        void Entity([NotNull] Type entityType, Action<ICosmosDBEntityModelBuilder> buildAction = null);

        IReadOnlyList<ICosmosDBEntityModel> GetEntities();
    }
}