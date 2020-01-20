using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Data;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Modularity;
using Volo.Abp.TestApp;
using Volo.Abp.TestApp.Application.Dto;
using Volo.Abp.TestApp.CosmosDB;
using Volo.Abp.TestApp.Domain;
using Volo.Abp.Threading;

namespace Volo.Abp.CosmosDB
{
    [DependsOn(
        typeof(AbpCosmosDBModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpAutofacModule),
        typeof(AbpTestBaseModule),
        typeof(AbpAutoMapperModule)
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

            context.Services.AddCosmosDBContext<TestAppCosmosDBContext>(options =>
            {
                options.AddDefaultRepositories<ITestAppCosmosDBContext>(true);
                options.AddRepository<City, CityRepository>();
            });

            ConfigureAutoMapper();
            ConfigureDistributedEventBus();
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            SeedTestData(context);
        }

        private void ConfigureAutoMapper()
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.Configurators.Add(ctx =>
                {
                    ctx.MapperConfiguration.CreateMap<Person, PersonDto>().ReverseMap();
                    ctx.MapperConfiguration.CreateMap<Phone, PhoneDto>().ReverseMap();
                });

                options.AddMaps<AbpCosmosDBTestModule>();
            });
        }

        private void ConfigureDistributedEventBus()
        {
            Configure<AbpDistributedEventBusOptions>(options =>
            {
                options.EtoMappings.Add<Person, PersonEto>();
            });
        }

        private static void SeedTestData(ApplicationInitializationContext context)
        {
            using (var scope = context.ServiceProvider.CreateScope())
            {
                AsyncHelper.RunSync(() => scope.ServiceProvider
                    .GetRequiredService<TestDataBuilder>()
                    .BuildAsync());
            }
        }
    }
}