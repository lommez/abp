namespace Volo.Abp.CosmosDB
{
    public interface ICosmosDBModelSource
    {
        CosmosDBContextModel GetModel(AbpCosmosDBContext dbContext);
    }
}