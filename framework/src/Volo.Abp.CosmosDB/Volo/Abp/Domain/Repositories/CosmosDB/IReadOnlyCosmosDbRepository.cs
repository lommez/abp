using System;
using System.Linq;
using System.Linq.Expressions;
using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abp.Domain.Repositories.CosmosDB
{
    public interface IReadOnlyCosmosDbRepository<TEntity> : IQueryable<TEntity>, IReadOnlyBasicCosmosDbRepository<TEntity>
        where TEntity : class, ICosmosDbEntity
    {
        IQueryable<TEntity> WithDetails();

        IQueryable<TEntity> WithDetails(params Expression<Func<TEntity, object>>[] propertySelectors);
    }
}