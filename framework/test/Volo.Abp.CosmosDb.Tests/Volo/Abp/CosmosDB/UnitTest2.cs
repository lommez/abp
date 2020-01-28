using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.CosmosDB.Extensions;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TestApp.Application;
using Volo.Abp.TestApp.Domain;
using Volo.Abp.Threading;
using Xunit;

namespace Volo.Abp.CosmosDB
{
    public class UnitTest2 : CosmosDBTestBase
    {
        protected virtual CancellationToken GetCancellationToken(ICancellationTokenProvider cancellationTokenProvider, CancellationToken prefferedValue = default)
        {
            return cancellationTokenProvider.FallbackToProvider(prefferedValue);
        }

        [Fact]
        public async Task Test2()
        {
            var connectionStringResolver = GetRequiredService<IConnectionStringResolver>();
            var defaultConnectionString = connectionStringResolver.Resolve("Default");
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
            var personContainer = database.GetContainer("Person");
            var dataFilter = GetRequiredService<IDataFilter>();
            var currentTenant = GetRequiredService<ICurrentTenant>();
            var cancellationTokenProvider = GetRequiredService<ICancellationTokenProvider>();            

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

            Stopwatch sw = new Stopwatch();
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
        }

        //[Fact]
        //public async Task Test3()
        //{
        //    var personAppService = GetRequiredService<IPersonAppService>();

        //    Stopwatch sw1 = new Stopwatch();
        //    sw1.Start();
        //    var people1 = await personAppService.GetListAsyncWithoutUOW();
        //    sw1.Stop();

        //    Stopwatch sw2 = new Stopwatch();
        //    sw2.Start();
        //    var people2 = await personAppService.GetListAsync();
        //    sw2.Stop();
        //}
    }
}