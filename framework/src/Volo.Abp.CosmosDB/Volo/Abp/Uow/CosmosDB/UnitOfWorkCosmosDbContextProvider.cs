using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Volo.Abp.CosmosDB;
using Volo.Abp.CosmosDB.Volo.Abp.CosmosDB.Json;
using Volo.Abp.Data;
using Volo.Abp.Uow.CosmosDB;

namespace Volo.Abp.Uow.MongoDB
{
    public class UnitOfWorkCosmosDBContextProvider<TCosmosDBContext> : ICosmosDBContextProvider<TCosmosDBContext>
        where TCosmosDBContext : IAbpCosmosDBContext
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IConnectionStringResolver _connectionStringResolver;

        public UnitOfWorkCosmosDBContextProvider(
            IUnitOfWorkManager unitOfWorkManager,
            IConnectionStringResolver connectionStringResolver)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _connectionStringResolver = connectionStringResolver;
        }

        public TCosmosDBContext GetDbContext()
        {
            var unitOfWork = _unitOfWorkManager.Current;
            if (unitOfWork == null)
            {
                throw new AbpException($"A CosmosDB context instance can only be created inside a unit of work!");
            }

            var connectionString = _connectionStringResolver.Resolve<TCosmosDBContext>();
            var dbContextKey = $"{typeof(TCosmosDBContext).FullName}_{connectionString}";

            var databaseName = ConnectionStringNameAttribute.GetConnStringName<TCosmosDBContext>();

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new AbpException("The CosmosDB context should have a ConnectionStringName to determine the name of the database.");
            }

            //TODO: Create only single MongoDbClient per connection string in an application (extract MongoClientCache for example).
            var databaseApi = unitOfWork.GetOrAddDatabaseApi(
                dbContextKey,
                () =>
                {
                    var serializerSettings = new JsonSerializerSettings()
                    {
                        ContractResolver = new ResolverWithPrivateSetters(),
                        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                    };

                    var clientOptions = new CosmosClientOptions
                    {
                        Serializer = new NewtonsoftJsonCosmosSerializer(serializerSettings)
                    };

                    var cosmosClient = new CosmosClient(connectionString, clientOptions);
                    var database = cosmosClient.GetDatabase(databaseName);
                    var dbContext = unitOfWork.ServiceProvider.GetRequiredService<TCosmosDBContext>();

                    dbContext.ToAbpCosmosDBContext().InitializeDatabase(cosmosClient, database);

                    return new CosmosDBDatabaseApi<TCosmosDBContext>(dbContext);
                });

            return ((CosmosDBDatabaseApi<TCosmosDBContext>)databaseApi).DbContext;
        }
    }
}