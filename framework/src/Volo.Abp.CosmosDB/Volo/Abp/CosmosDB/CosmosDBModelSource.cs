using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.CosmosDB;
using Volo.Abp.Reflection;

namespace Volo.Abp.CosmosDB
{
    public class CosmosDBModelSource : ICosmosDBModelSource, ISingletonDependency
    {
        protected readonly ConcurrentDictionary<Type, CosmosDBContextModel> ModelCache = new ConcurrentDictionary<Type, CosmosDBContextModel>();

        public virtual CosmosDBContextModel GetModel(AbpCosmosDBContext dbContext)
        {
            return ModelCache.GetOrAdd(
                dbContext.GetType(),
                _ => CreateModel(dbContext)
            );
        }

        protected virtual CosmosDBContextModel CreateModel(AbpCosmosDBContext dbContext)
        {
            var modelBuilder = CreateModelBuilder();
            BuildModelFromDbContextType(modelBuilder, dbContext.GetType());
            BuildModelFromDbContextInstance(modelBuilder, dbContext);
            return modelBuilder.Build();
        }

        protected virtual CosmosDBModelBuilder CreateModelBuilder()
        {
            return new CosmosDBModelBuilder();
        }

        protected virtual void BuildModelFromDbContextType(ICosmosDBModelBuilder modelBuilder, Type dbContextType)
        {
            var collectionProperties =
                from property in dbContextType.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where
                    ReflectionHelper.IsAssignableToGenericType(property.PropertyType, typeof(ICosmosDBCollection<>)) &&
                    typeof(ICosmosDBEntity).IsAssignableFrom(property.PropertyType.GenericTypeArguments[0])
                select property;

            foreach (var collectionProperty in collectionProperties)
            {
                BuildModelFromDbContextCollectionProperty(modelBuilder, collectionProperty);
            }
        }

        protected virtual void BuildModelFromDbContextCollectionProperty(ICosmosDBModelBuilder modelBuilder, PropertyInfo collectionProperty)
        {
            var entityType = collectionProperty.PropertyType.GenericTypeArguments[0];
            var collectionAttribute = collectionProperty.GetCustomAttributes().OfType<CosmosDBCollectionAttribute>().FirstOrDefault();

            modelBuilder.Entity(entityType, b =>
            {
                b.CollectionName = collectionAttribute?.CollectionName ?? collectionProperty.Name;
            });
        }

        protected virtual void BuildModelFromDbContextInstance(ICosmosDBModelBuilder modelBuilder, AbpCosmosDBContext dbContext)
        {
            dbContext.CreateModel(modelBuilder);
        }
    }
}