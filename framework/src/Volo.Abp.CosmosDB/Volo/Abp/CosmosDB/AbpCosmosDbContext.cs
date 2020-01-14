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

        public virtual ICosmosDBCollection<TEntity> Collection<TEntity>()
            where TEntity : class, ICosmosDBEntity
        {
            var collectionName = GetCollectionName<TEntity>();
            var collection = new CosmosDBCollection<TEntity>(Database, collectionName);
            return collection;
        }

        public virtual string GetCollectionName<TEntity>()
            where TEntity : class, ICosmosDBEntity
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