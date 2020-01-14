using System;
using System.Linq;
using System.Linq.Expressions;
using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abp.Domain.Repositories.CosmosDB
{
    public interface IReadOnlyCosmosDBRepository<TEntity> : IQueryable<TEntity>, IReadOnlyBasicCosmosDBRepository<TEntity>
        where TEntity : class, ICosmosDBEntity
    {
        IQueryable<TEntity> WithDetails();

        IQueryable<TEntity> WithDetails(params Expression<Func<TEntity, object>>[] propertySelectors);
    }
}