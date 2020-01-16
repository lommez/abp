using System;
using System.Linq;
using System.Linq.Expressions;
using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abp.Domain.Repositories.CosmosDB
{
    public interface IReadOnlyCosmosDBRepository<TEntity, TPartitionKeyType> : IQueryable<TEntity>, IReadOnlyBasicCosmosDBRepository<TEntity, TPartitionKeyType>
        where TEntity : class, ICosmosDBEntity<TPartitionKeyType>
    {
        IQueryable<TEntity> WithDetails();

        IQueryable<TEntity> WithDetails(params Expression<Func<TEntity, object>>[] propertySelectors);
    }
}