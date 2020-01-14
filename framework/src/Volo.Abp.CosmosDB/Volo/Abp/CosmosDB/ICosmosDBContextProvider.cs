namespace Volo.Abp.CosmosDB
{
    public interface ICosmosDBContextProvider<out TCosmosDBContext>
        where TCosmosDBContext : IAbpCosmosDBContext
    {
        TCosmosDBContext GetDbContext();
    }
}