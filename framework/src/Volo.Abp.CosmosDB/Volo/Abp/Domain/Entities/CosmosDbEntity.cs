using Newtonsoft.Json;

namespace Volo.Abp.Domain.Entities.CosmosDB
{
    public abstract class CosmosDBEntity<TPartitionKeyType> : Entity<string>, ICosmosDBEntity<TPartitionKeyType>
    {
        [JsonIgnore]
        public abstract TPartitionKeyType PartitionKeyValue { get; }

        [JsonProperty("_rid")]
        public string _rid { get; protected set; }

        [JsonProperty("_self")]
        public string _self { get; protected set; }

        [JsonProperty("_etag")]
        public string _etag { get; protected set; }

        [JsonProperty("_attachments")]
        public string _attachments { get; protected set; }

        [JsonProperty("_ts")]
        public long _ts { get; protected set; }
    }
}