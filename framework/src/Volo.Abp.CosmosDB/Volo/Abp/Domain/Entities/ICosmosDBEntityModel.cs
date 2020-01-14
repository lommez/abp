using System;

namespace Volo.Abp.Domain.Entities.CosmosDB
{
    public interface ICosmosDBEntityModel
    {
        Type EntityType { get; }

        string CollectionName { get; }
    }
}