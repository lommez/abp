using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using Volo.Abp.CosmosDB;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories.CosmosDB;
using Volo.Abp.TestApp.CosmosDB;
using Volo.Abp.TestApp.Domain;
using Xunit;

namespace Volo.Abo.CosmosDB.Tests
{
    public class UnitTest1 : CosmosDBTestBase
    {
        [Fact]
        public async Task Test1()
        {
            try
            {
                var ofertaRepository = GetRequiredService<ICosmosDBRepository<Oferta, string>>();
                var ddd = await ofertaRepository.FindAsync("e58c20c9-dc78-4f2a-a6a2-b510a0831ea9", "lommez");
            }
            catch (System.Exception e)
            {
                throw;
            }

            var connectionStringResolver = GetRequiredService<IConnectionStringResolver>();
            var defaultConnectionString = connectionStringResolver.Resolve("Default");
            //var serializerSettings = new JsonSerializerSettings()
            //{
            //    ContractResolver = new ResolverWithPrivateSetters(),
            //    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            //};
            var clientOptions = new CosmosClientOptions
            {
                //Serializer = new NewtonsoftJsonCosmosSerializer(serializerSettings),
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            };
            var cosmosClient = new CosmosClient(defaultConnectionString, clientOptions);
            await cosmosClient.CreateDatabaseIfNotExistsAsync("Ofertas");
            var database = cosmosClient.GetDatabase("Ofertas");
            var ofertasContainer = database.GetContainer("OfertasContainer");
            var ofertasCollection = new CosmosDBCollection<Oferta, string>(database, "OfertasContainer");
            var teste = await ofertasCollection.ReadDocumentAsync(
                "e58c20c9-dc78-4f2a-a6a2-b510a0831ea9",
                 new PartitionKey("lommez"));

            //var unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();
            //var unitOfWork = GetRequiredService<ICosmosDBContextProvider<TestAppCosmosDBContext>>();

            //using (var uow = unitOfWorkManager.Begin())
            //{
            //    var context = unitOfWork.GetDbContext();
            //    await uow.CompleteAsync();
            //}
            //var aaa = new TestAppCosmosDBContext();
            //var builder = new CosmosDBModelBuilder();
            //aaa.ModelSource = new CosmosDBModelSource();
            //var bbb = aaa.GetEntityModel<Oferta>();
            //var xxxx = await aaa.Oferta.ReadDocumentAsync(
            //    "e58c20c9-dc78-4f2a-a6a2-b510a0831ea9",
            //     new PartitionKey("lommez"));
        }
    }
}