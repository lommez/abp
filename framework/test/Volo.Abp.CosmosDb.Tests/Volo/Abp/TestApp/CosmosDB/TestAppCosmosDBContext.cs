using Volo.Abp.CosmosDB;
using Volo.Abp.TestApp.Domain;

namespace Volo.Abp.TestApp.CosmosDB
{
    public class TestAppCosmosDBContext : AbpCosmosDBContext, ITestAppCosmosDBContext
    {
        //[CosmosDBCollection("Persons")] //Intentionally changed the collection name to test it
        public ICosmosDBCollection<Person, string> People => Collection<Person, string>();

        public ICosmosDBCollection<City, string> Cities => Collection<City, string>();

        public TestAppCosmosDBContext(ICosmosDBClientFactory cosmosDBClientFactory)
            : base(cosmosDBClientFactory)
        {

        }

        protected override void CreateModel(ICosmosDBModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);

            modelBuilder.Entity<City>(b =>
            {
                b.CollectionName = "City";
            });

            modelBuilder.Entity<Person>(b =>
            {
                b.CollectionName = "Person";
            });
        }
    }
}