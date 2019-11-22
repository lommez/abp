namespace Volo.Abp.Domain.Entities.CosmosDB
{
    public interface ICosmosDbEntity : IEntity<string>
    {
        string PartitionKey { get; set; }
    }
}
