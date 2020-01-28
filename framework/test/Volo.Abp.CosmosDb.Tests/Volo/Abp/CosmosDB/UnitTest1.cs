using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.CosmosDB;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories.CosmosDB;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TestApp.CosmosDB;
using Volo.Abp.TestApp.Domain;
using Xunit;
using Volo.Abp.CosmosDB.Extensions;
using Volo.Abp.Threading;
using System.Collections.Generic;

namespace Volo.Abo.CosmosDB.Tests
{
    public class UnitTest1 : CosmosDBTestBase
    {
        protected virtual CancellationToken GetCancellationToken(ICancellationTokenProvider cancellationTokenProvider, CancellationToken prefferedValue = default)
        {
            return cancellationTokenProvider.FallbackToProvider(prefferedValue);
        }

        [Fact]
        public void ResolveRepository()
        {
            var repo = GetRequiredService<ICosmosDBRepository<Person, string>>();
        }

        [Fact]
        public async Task Test1()
        {
            //try
            //{
            //    var ofertaRepository = GetRequiredService<ICosmosDBRepository<Oferta, string>>();
            //    var ddd = await ofertaRepository.FindAsync("e58c20c9-dc78-4f2a-a6a2-b510a0831ea9", "lommez");
            //}
            //catch (System.Exception e)
            //{
            //    throw;
            //}

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
            await cosmosClient.CreateDatabaseIfNotExistsAsync("DemoDb");
            var database = cosmosClient.GetDatabase("DemoDb");            
            var personContainer = database.GetContainer("Person");
            var dataFilter = GetRequiredService<IDataFilter>();
            var currentTenant = GetRequiredService<ICurrentTenant>();
            var cancellationTokenProvider = GetRequiredService<ICancellationTokenProvider>();

            Stopwatch sw = new Stopwatch();                        

            Func<IQueryable<Person>, IQueryable<Person>> ApplyDataFilters = (query) =>
            {
                if (typeof(ISoftDelete).IsAssignableFrom(typeof(Person)))
                {
                    query = query.WhereIf(dataFilter.IsEnabled<ISoftDelete>(), e => ((ISoftDelete)e).IsDeleted == false);
                }

                if (typeof(IMultiTenant).IsAssignableFrom(typeof(Person)))
                {
                    var tenantId = currentTenant.Id;
                    query = query.WhereIf(dataFilter.IsEnabled<IMultiTenant>(), e => ((IMultiTenant)e).TenantId == tenantId);
                }

                return query;
            };

            var query = ApplyDataFilters(personContainer.GetItemLinqQueryable<Person>());
            sw.Start();
            var iterator = query.ToFeedIterator();
            var data = iterator.AsAsyncEnumerable(GetCancellationToken(cancellationTokenProvider));
            var list = new List<Person>();
            await foreach (var item in data)
            {
                list.Add(item);
            }
            sw.Stop();
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