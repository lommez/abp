using Volo.Abp.DependencyInjection;

namespace Volo.Abp.CosmosDB
{
    public interface ICosmosDbClientFactory : ITransientDependency
    {
        ICosmosDbClient GetClient(string collectionName);
    }
}