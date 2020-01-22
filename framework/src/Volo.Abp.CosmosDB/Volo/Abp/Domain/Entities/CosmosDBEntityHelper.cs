using JetBrains.Annotations;
using System;
using System.Reflection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abp.CosmosDB.Volo.Abp.Domain.Entities
{
    /// <summary>
    /// Some helper methods for CosmosDB Entities.
    /// </summary>
    public static class CosmosDBEntityHelper
    {
        /// <summary>
        /// Tries to find the partition key type of the given CosmosDB entity type.
        /// May return null if given type does not implement <see cref="ICosmosDBEntity{TKey}"/>
        /// </summary>
        [CanBeNull]
        public static Type FindPartitionKeyType([NotNull] Type entityType)
        {
            if (!typeof(IEntity).IsAssignableFrom(entityType))
            {
                throw new AbpException($"Given {nameof(entityType)} is not an entity. It should implement {typeof(IEntity).AssemblyQualifiedName}!");
            }

            foreach (var interfaceType in entityType.GetTypeInfo().GetInterfaces())
            {
                if (interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(ICosmosDBEntity<>))
                {
                    return interfaceType.GenericTypeArguments[0];
                }
            }

            return null;
        }
    }
}