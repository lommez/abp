using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abp.TestApp.Domain
{
    public class Oferta : CosmosDBEntity
    {
        public string Nome { get; set; }

        public string UserId { get; set; }
    }
}