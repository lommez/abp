using Volo.Abp.CosmosDB;
using Volo.Abp.Data;
using Volo.Abp.TestApp.Domain;

namespace Volo.Abp.TestApp.CosmosDB
{
    [ConnectionStringName("DemoDb")]
    public interface ITestAppCosmosDBContext : IAbpCosmosDBContext
    {
        ICosmosDBCollection<Person, string> People { get; }

        ICosmosDBCollection<City, string> Cities { get; }
    }
}