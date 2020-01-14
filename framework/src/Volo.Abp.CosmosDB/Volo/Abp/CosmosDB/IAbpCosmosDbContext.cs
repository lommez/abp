using Microsoft.Azure.Cosmos;
using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abp.CosmosDB
{
    public interface IAbpCosmosDBContext
    {
        Database Database { get; }

        CosmosClient CosmosClient { get; }

        ICosmosDBCollection<TEntity> Collection<TEntity>()
            where TEntity : class, ICosmosDBEntity;
    }
}