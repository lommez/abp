using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abp.TestApp.Domain
{
    public class Oferta : CosmosDBEntity<string>
    {
        public string Nome { get; set; }

        public string UserId { get; set; }

        public override string PartitionKeyValue => UserId;
    }
}