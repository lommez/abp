namespace Volo.Abp.CosmosDB
{
    public static class AbpCosmosDBContextExtensions
    {
        public static AbpCosmosDBContext ToAbpCosmosDBContext(this IAbpCosmosDBContext dbContext)
        {
            var abpCosmosDBContext = dbContext as AbpCosmosDBContext;

            if (abpCosmosDBContext == null)
            {
                throw new AbpException($"The type {dbContext.GetType().AssemblyQualifiedName} should be convertable to {typeof(AbpCosmosDBContext).AssemblyQualifiedName}!");
            }

            return abpCosmosDBContext;
        }
    }
}