using Volo.Abp.CosmosDB;
using Volo.Abp.TestApp.Domain;

namespace Volo.Abp.TestApp.CosmosDB
{
    public class TestAppCosmosDBContext : AbpCosmosDBContext, ITestAppCosmosDBContext
    {
        //[CosmosDBCollection("OfertasContainer")] //Intentionally changed the collection name to test it
        public ICosmosDBCollection<Oferta, string> Oferta => Collection<Oferta, string>();

        protected override void CreateModel(ICosmosDBModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);

            modelBuilder.Entity<Oferta>(b =>
            {
                b.CollectionName = "OfertasContainer";
            });
        }
    }
}