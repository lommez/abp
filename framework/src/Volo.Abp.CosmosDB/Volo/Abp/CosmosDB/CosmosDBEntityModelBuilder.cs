using System;
using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abp.CosmosDB
{
    public class CosmosDBEntityModelBuilder<TEntity> :
        ICosmosDBEntityModel,
        ICosmosDBEntityModelBuilder,
        ICosmosDBEntityModelBuilder<TEntity>
    {
        public Type EntityType { get; }

        public string CollectionName { get; set; }

        public CosmosDBEntityModelBuilder()
        {
            EntityType = typeof(TEntity);
        }
    }
}