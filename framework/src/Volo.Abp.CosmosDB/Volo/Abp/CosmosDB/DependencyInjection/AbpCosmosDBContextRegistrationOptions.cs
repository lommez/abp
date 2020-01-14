using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.DependencyInjection;

namespace Volo.Abp.CosmosDB.DependencyInjection
{
    public class AbpCosmosDBContextRegistrationOptions : AbpCommonDbContextRegistrationOptions, IAbpCosmosDBContextRegistrationOptionsBuilder
    {
        public AbpCosmosDBContextRegistrationOptions(Type originalDbContextType, IServiceCollection services)
            : base(originalDbContextType, services)
        {
        }
    }
}