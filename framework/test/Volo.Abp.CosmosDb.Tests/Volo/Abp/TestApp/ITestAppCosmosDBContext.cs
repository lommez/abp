using Volo.Abp.CosmosDB;
using Volo.Abp.Data;
using Volo.Abp.TestApp.Domain;

namespace Volo.Abp.TestApp.CosmosDB
{
    [ConnectionStringName("OfertaContainer")]
    public interface ITestAppCosmosDBContext : IAbpCosmosDBContext
    {
        ICosmosDBCollection<Oferta> Oferta { get; }
    }
}