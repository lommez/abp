using Newtonsoft.Json;

namespace Volo.Abp.Domain.Entities.CosmosDB
{
    public abstract class CosmosDBEntity<TPartitionKeyType> : Entity<string>, ICosmosDBEntity<TPartitionKeyType>
    {
        [JsonIgnore]
        public abstract TPartitionKeyType PartitionKeyValue { get; }
    }
}