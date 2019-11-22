namespace Volo.Abp.Domain.Entities.CosmosDB
{
    public class CosmosDbEntity : Entity<string>, ICosmosDbEntity
    {
        public string PartitionKey { get; set; }
    }
}