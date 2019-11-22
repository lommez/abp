using Microsoft.Azure.Documents;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.CosmosDB
{
    public interface ICosmosDbDocumentClientFactory : ITransientDependency
    {
        IDocumentClient CreateDocumentClient();
    }
}