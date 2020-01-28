using Microsoft.Azure.Cosmos;

namespace Volo.Abp.CosmosDB
{
    public interface ICosmosDBClientFactory
    {
        CosmosClient GetClient<TCosmosDBContext>(TCosmosDBContext dbContext)
            where TCosmosDBContext : class, IAbpCosmosDBContext;

        Database GetDatabase();
    }
}