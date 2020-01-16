namespace Volo.Abp.Domain.Entities.CosmosDB
{
    public abstract class CosmosDBEntity<TPartitionKeyType> : Entity<string>, ICosmosDBEntity<TPartitionKeyType>
    {
        public abstract TPartitionKeyType PartitionKeyValue { get; }
    }
}