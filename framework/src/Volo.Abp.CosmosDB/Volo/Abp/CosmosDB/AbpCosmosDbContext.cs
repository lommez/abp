using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abp.CosmosDB
{
    public abstract class AbpCosmosDBContext : IAbpCosmosDBContext, ITransientDependency
    {
        public ICosmosDBModelSource ModelSource { get; set; }

        public Database Database { get; private set; }

        public CosmosClient CosmosClient { get; private set; }

        protected internal virtual void CreateModel(ICosmosDBModelBuilder modelBuilder)
        {
        }

        public virtual void InitializeDatabase(CosmosClient cosmosClient, Database database)
        {
            CosmosClient = cosmosClient;
            Database = database;
        }

        public virtual ICosmosDBCollection<TEntity, TPartitionKeyType> Collection<TEntity, TPartitionKeyType>()
            where TEntity : class, ICosmosDBEntity<TPartitionKeyType>
        {
            var collectionName = GetCollectionName<TEntity, TPartitionKeyType>();
            var collection = new CosmosDBCollection<TEntity, TPartitionKeyType>(Database, collectionName);
            return collection;
        }

        public virtual string GetCollectionName<TEntity, TPartitionKeyType>()
            where TEntity : class, ICosmosDBEntity<TPartitionKeyType>
        {
            return GetEntityModel<TEntity>().CollectionName;
        }

        public virtual ICosmosDBEntityModel GetEntityModel<TEntity>()
        {
            var model = ModelSource.GetModel(this).Entities.GetOrDefault(typeof(TEntity));

            if (model == null)
            {
                throw new AbpException("Could not find a model for given entity type: " + typeof(TEntity).AssemblyQualifiedName);
            }

            return model;
        }
    }
}