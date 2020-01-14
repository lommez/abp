using System;

namespace Volo.Abp.CosmosDB
{
    public interface ICosmosDBEntityModelBuilder<TEntity>
    {
        Type EntityType { get; }

        string CollectionName { get; set; }
    }

    public interface ICosmosDBEntityModelBuilder
    {
        Type EntityType { get; }

        string CollectionName { get; set; }
    }
}