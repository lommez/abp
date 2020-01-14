using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using Volo.Abp.CosmosDB;
using Volo.Abp.CosmosDB.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AbpCosmosDBServiceCollectionExtensions
    {
        public static IServiceCollection AddCosmosDBContext<TCosmosDBContext>(this IServiceCollection services, Action<IAbpCosmosDBContextRegistrationOptionsBuilder> optionsBuilder = null) //Created overload instead of default parameter
            where TCosmosDBContext : AbpCosmosDBContext
        {
            var options = new AbpCosmosDBContextRegistrationOptions(typeof(TCosmosDBContext), services);
            optionsBuilder?.Invoke(options);

            foreach (var dbContextType in options.ReplacedDbContextTypes)
            {
                services.Replace(ServiceDescriptor.Transient(dbContextType, typeof(TCosmosDBContext)));
            }

            var registrar = new CosmosDBRepositoryRegistrar(options);
            registrar.AddRepositories();
            registrar.AddCosmosDbRepositories();

            return services;
        }
    }
}