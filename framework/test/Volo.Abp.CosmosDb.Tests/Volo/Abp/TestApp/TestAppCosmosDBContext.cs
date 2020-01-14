using Volo.Abp.CosmosDB;
using Volo.Abp.Data;
using Volo.Abp.TestApp.Domain;

namespace Volo.Abp.TestApp.CosmosDB
{
    [ConnectionStringName("Ofertas")]
    public class TestAppCosmosDBContext : AbpCosmosDBContext, ITestAppCosmosDBContext
    {
        //[CosmosDBCollection("OfertasContainer")] //Intentionally changed the collection name to test it
        public ICosmosDBCollection<Oferta> Oferta
        {
            get
            {
                return Collection<Oferta>();
            }
        }

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