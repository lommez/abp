using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Volo.Abp.Domain.Entities.CosmosDB;
using Volo.Abp.Reflection;

namespace Volo.Abp.CosmosDB
{
    internal static class CosmosDBContextHelper
    {
        public static IEnumerable<Type> GetEntityTypes(Type dbContextType)
        {
            return
                from property in dbContextType.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where
                    ReflectionHelper.IsAssignableToGenericType(property.PropertyType, typeof(ICosmosDBCollection<>)) &&
                    typeof(ICosmosDBEntity).IsAssignableFrom(property.PropertyType.GenericTypeArguments[0])
                select property.PropertyType.GenericTypeArguments[0];
        }
    }
}