namespace Volo.Abp.CosmosDB
{
    public abstract class CosmosDBTestBase : AbpIntegratedTest<AbpCosmosDBTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}