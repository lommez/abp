using Volo.Abp.Domain.Entities;

namespace Volo.Abp.CosmosDB
{
    public interface ICosmosDbCollection<TDocument>
        where TDocument: class, IEntity<string>
    {
    }
}
