namespace Volo.Abp.Domain.Entities.CosmosDB
{
    public interface ICosmosDBEntity<TPartitionKeyType> : IEntity<string>
    {
        TPartitionKeyType PartitionKeyValue { get; }
    }
}