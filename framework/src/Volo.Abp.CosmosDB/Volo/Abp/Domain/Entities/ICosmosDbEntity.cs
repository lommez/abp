namespace Volo.Abp.Domain.Entities.CosmosDB
{
    public interface ICosmosDBEntity<TPartitionKeyType> : IEntity<string>
    {
        TPartitionKeyType PartitionKeyValue { get; }

        string _rid { get; }

        string _self { get; }

        string _etag { get; }

        string _attachments { get; }

        long _ts { get; }
    }
}