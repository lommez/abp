//using Microsoft.Azure.Cosmos;
//using Microsoft.Azure.Cosmos.Linq;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Volo.Abp.CosmosDB;
//using Volo.Abp.CosmosDB.Extensions;
//using Volo.Abp.Data;
//using Volo.Abp.DependencyInjection;
//using Volo.Abp.MultiTenancy;
//using Volo.Abp.TestApp.CosmosDB;
//using Volo.Abp.TestApp.Domain;
//using Volo.Abp.Threading;
//using Volo.Abp.Uow;

//namespace Volo.Abp.TestApp.Application
//{
//    public interface IPersonAppService : ITransientDependency
//    {
//        Task<List<Person>> GetListAsync();

//        Task<List<Person>> GetListAsyncWithoutUOW();
//    }

//    public class PersonAppService : IPersonAppService
//    {
//        private readonly IUnitOfWorkManager _unitOfWorkManager;
//        private readonly ICosmosDBContextProvider<ITestAppCosmosDBContext> _cosmosDBContextProvider;
//        private readonly IDataFilter _dataFilter;
//        private readonly ICancellationTokenProvider _cancellationTokenProvider;
//        private readonly ICurrentTenant _currentTenant;
//        private readonly Database _database;
//        private readonly Container _personContainer;

//        public PersonAppService(
//            IUnitOfWorkManager unitOfWorkManager,
//            ICosmosDBContextProvider<ITestAppCosmosDBContext> cosmosDBContextProvider,
//            IDataFilter dataFilter,
//            ICancellationTokenProvider cancellationTokenProvider,
//            IConnectionStringResolver connectionStringResolver,
//            ICurrentTenant currentTenant)
//        {
//            _unitOfWorkManager = unitOfWorkManager;
//            _cosmosDBContextProvider = cosmosDBContextProvider;
//            _dataFilter = dataFilter;
//            _cancellationTokenProvider = cancellationTokenProvider;
//            _currentTenant = currentTenant;

//            var defaultConnectionString = connectionStringResolver.Resolve("Default");
//            var clientOptions = new CosmosClientOptions
//            {
//                SerializerOptions = new CosmosSerializationOptions
//                {
//                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
//                }
//            };
//            var cosmosClient = new CosmosClient(defaultConnectionString, clientOptions);
//            cosmosClient.CreateDatabaseIfNotExistsAsync("DemoDb").GetAwaiter().GetResult();
//            _database = cosmosClient.GetDatabase("DemoDb");
//            _personContainer = _database.GetContainer("Person");
//        }

//        public async Task<List<Person>> GetListAsync()
//        {
//            Stopwatch sw1 = new Stopwatch();
//            Stopwatch sw2 = new Stopwatch();

//            using (var uow = _unitOfWorkManager.Begin())
//            {
//                sw1.Start();

//                var context = _cosmosDBContextProvider.GetDbContext();
//                var personContainer = context.Database.GetContainer("Person");

//                var query = ApplyDataFilters(personContainer.GetItemLinqQueryable<Person>());
//                var iterator = query.ToFeedIterator();
//                var data = iterator.AsAsyncEnumerable(GetCancellationToken(_cancellationTokenProvider));
//                var list = new List<Person>();

//                sw1.Stop();
//                sw2.Start();

//                await foreach (var item in data)
//                {
//                    list.Add(item);
//                }

//                sw2.Stop();

//                return list;
//            }
//        }

//        public async Task<List<Person>> GetListAsyncWithoutUOW()
//        {


//            var query = ApplyDataFilters(_personContainer.GetItemLinqQueryable<Person>());
//            var iterator = query.ToFeedIterator();
//            var data = iterator.AsAsyncEnumerable(GetCancellationToken(_cancellationTokenProvider));
//            var list = new List<Person>();

//            Stopwatch sw = new Stopwatch();
//            sw.Start();

//            await foreach (var item in data)
//            {
//                list.Add(item);
//            }

//            sw.Stop();

//            return list;
//        }

//        protected virtual TQueryable ApplyDataFilters<TQueryable>(TQueryable query)
//            where TQueryable : IQueryable<Person>
//        {
//            if (typeof(ISoftDelete).IsAssignableFrom(typeof(Person)))
//            {
//                query = (TQueryable)query.WhereIf(_dataFilter.IsEnabled<ISoftDelete>(), e => ((ISoftDelete)e).IsDeleted == false);
//            }

//            if (typeof(IMultiTenant).IsAssignableFrom(typeof(Person)))
//            {
//                var tenantId = _currentTenant.Id;
//                query = (TQueryable)query.WhereIf(_dataFilter.IsEnabled<IMultiTenant>(), e => ((IMultiTenant)e).TenantId == tenantId);
//            }

//            return query;
//        }

//        protected virtual CancellationToken GetCancellationToken(ICancellationTokenProvider cancellationTokenProvider, CancellationToken prefferedValue = default)
//        {
//            return cancellationTokenProvider.FallbackToProvider(prefferedValue);
//        }
//    }
//}