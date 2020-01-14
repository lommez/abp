using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories.CosmosDB;
using Volo.Abp.Modularity;
using Volo.Abp.TestApp.CosmosDB;
using Volo.Abp.TestApp.Domain;

namespace Volo.Abp.CosmosDB
{
    [DependsOn(
        typeof(AbpCosmosDBModule),
        //typeof(AbpDddApplicationModule),
        typeof(AbpAutofacModule)
        //typeof(AbpTestBaseModule),
        //typeof(AbpAutoMapperModule)
        )]
    public class AbpCosmosDBTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var connectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

            Configure<AbpDbConnectionOptions>(options =>
            {
                options.ConnectionStrings.Default = connectionString;
            });

            //context.Services.AddTransient(typeof(ICosmosDBRepository<Oferta>), typeof(CosmosDBRepository<TestAppCosmosDBContext, Oferta>));
            context.Services.AddCosmosDBContext<TestAppCosmosDBContext>(options =>
            {
                options.AddDefaultRepositories<ITestAppCosmosDBContext>(true);
            });
        }
    }
}