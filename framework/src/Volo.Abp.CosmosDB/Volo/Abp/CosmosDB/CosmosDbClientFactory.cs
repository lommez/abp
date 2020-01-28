using Microsoft.Azure.Cosmos;
using System;
using System.Reflection;
using Volo.Abp.Data;

namespace Volo.Abp.CosmosDB
{
    public class CosmosDBClientFactory : ICosmosDBClientFactory
    {
        private readonly IConnectionStringResolver _connectionStringResolver;
        private readonly CosmosClientOptions _cosmosClientOptions;
        private CosmosClient _cosmosClient;
        private Database _database;

        private static readonly object SyncObj = new object();

        public CosmosDBClientFactory(IConnectionStringResolver connectionStringResolver, CosmosClientOptions cosmosClientOptions)
        {
            _connectionStringResolver = connectionStringResolver ?? throw new ArgumentNullException(nameof(connectionStringResolver));
            _cosmosClientOptions = cosmosClientOptions ?? throw new ArgumentNullException(nameof(cosmosClientOptions));
        }

        public CosmosClient GetClient<TCosmosDBContext>(TCosmosDBContext dbContext)
            where TCosmosDBContext : class, IAbpCosmosDBContext
        {
            if (_cosmosClient == null)
            {
                lock (SyncObj)
                {
                    var type = GetTypeWithConnectionStringNameAttribute(dbContext);
                    var databaseName = ConnectionStringNameAttribute.GetConnStringName(type);
                    var connectionString = _connectionStringResolver.Resolve(databaseName);
                    _cosmosClient = new CosmosClient(connectionString, _cosmosClientOptions);
                    _database = _cosmosClient.GetDatabase(databaseName);
                }
            }

            return _cosmosClient;
        }

        public Database GetDatabase()
        {
            return _database;
        }

        private Type GetTypeWithConnectionStringNameAttribute(IAbpCosmosDBContext context)
        {
            var type = context.GetType();
            var nameAttribute = type.GetTypeInfo().GetCustomAttribute<ConnectionStringNameAttribute>();

            if (nameAttribute == null)
            {
                foreach (var @interface in type.GetInterfaces())
                {
                    if (@interface.GetTypeInfo().GetCustomAttribute<ConnectionStringNameAttribute>() != null)
                    {
                        return @interface;
                    }
                }

                throw new AbpException("The CosmosDB context or its interface should be decorated with ConnectionStringName attribute.");
            }
            else
            {
                return type;
            }
        }
    }
}