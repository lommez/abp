using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.CosmosDB;
using Volo.Abp.CosmosDB.Volo.Abp.CosmosDB.Json;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories.CosmosDB;
using Volo.Abp.TestApp.CosmosDB;
using Volo.Abp.TestApp.Domain;
using Volo.Abp.Uow;
using Xunit;
using System.Linq;
using System.Reflection;
using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abo.CosmosDB.Tests
{
    public class Teste1 : CosmosDBEntity
    {
        public int MyProperty { get; set; }

        public string MyProperty2 { get; set; }

        public Teste1()
        {
            this.SetPartititionKeyProperty<Teste1>(x => x.MyProperty2);
        }
    }

    public class UnitTest1 : CosmosDBTestBase
    {
        [Fact]
        public void ExpressionTest()
        {
            var teste = new Teste1();
            teste.MyProperty = 1;
            teste.MyProperty2 = "2";
            var value = teste.GetPartititionKeyValue();

        }
        [Fact]
        public async Task Test1()
        {
            try
            {
                var ofertaRepository = GetRequiredService<ICosmosDBRepository<Oferta>>();
                var ddd = ofertaRepository.FindAsync("e58c20c9-dc78-4f2a-a6a2-b510a0831ea9").GetAwaiter().GetResult();
            }
            catch (System.Exception e)
            {

                throw;
            }

            var connectionStringResolver = GetRequiredService<IConnectionStringResolver>();
            var defaultConnectionString = connectionStringResolver.Resolve("Default");
            var serializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new ResolverWithPrivateSetters(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            };
            var clientOptions = new CosmosClientOptions
            {
                Serializer = new NewtonsoftJsonCosmosSerializer(serializerSettings)
            };
            var cosmosClient = new CosmosClient(defaultConnectionString, clientOptions);
            await cosmosClient.CreateDatabaseIfNotExistsAsync("Ofertas");
            var database = cosmosClient.GetDatabase("Ofertas");
            var ofertasContainer = database.GetContainer("OfertasContainer");
            var ofertasCollection = new CosmosDBCollection<Oferta>(database, "OfertasContainer");
            var teste = await ofertasCollection.ReadDocumentAsync(
                "e58c20c9-dc78-4f2a-a6a2-b510a0831ea9",
                 new PartitionKey("lommez"));

            var unitOfWorkManager = GetRequiredService<IUnitOfWorkManager>();
            var unitOfWork = GetRequiredService<ICosmosDBContextProvider<TestAppCosmosDBContext>>();

            using (var uow = unitOfWorkManager.Begin())
            {
                var context = unitOfWork.GetDbContext();
                await uow.CompleteAsync();
            }
            var aaa = new TestAppCosmosDBContext();
            var builder = new CosmosDBModelBuilder();
            aaa.ModelSource = new CosmosDBModelSource();
            var bbb = aaa.GetEntityModel<Oferta>();
            var xxxx = await aaa.Oferta.ReadDocumentAsync(
                "e58c20c9-dc78-4f2a-a6a2-b510a0831ea9",
                 new PartitionKey("lommez"));

        }
    }
}