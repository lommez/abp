namespace Volo.Abp.Uow.CosmosDB
{
    public class CosmosDBDatabaseApi<TCosmosDBContext> : IDatabaseApi
    {
        public TCosmosDBContext DbContext { get; }

        public CosmosDBDatabaseApi(TCosmosDBContext dbContext)
        {
            DbContext = dbContext;
        }
    }
}
