using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.Uow.CosmosDB;

namespace Volo.Abp.CosmosDB
{
    [DependsOn(typeof(AbpDddDomainModule))]
    public class AbpCosmosDBModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.TryAddTransient(
                typeof(ICosmosDBContextProvider<>),
                typeof(UnitOfWorkCosmosDBContextProvider<>)
            );
        }
    }
}