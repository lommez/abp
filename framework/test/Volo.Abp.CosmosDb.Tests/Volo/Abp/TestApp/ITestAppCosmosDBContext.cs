using Volo.Abp.CosmosDB;
using Volo.Abp.Data;
using Volo.Abp.TestApp.Domain;

namespace Volo.Abp.TestApp.CosmosDB
{
    [ConnectionStringName("Ofertas")]
    public interface ITestAppCosmosDBContext : IAbpCosmosDBContext
    {
        ICosmosDBCollection<Oferta, string> Oferta { get; }
    }
}