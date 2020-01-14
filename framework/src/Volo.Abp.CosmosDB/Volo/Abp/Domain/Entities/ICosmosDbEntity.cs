using System;
using System.Linq.Expressions;

namespace Volo.Abp.Domain.Entities.CosmosDB
{
    public interface ICosmosDBEntity : IEntity<string>
    {
        string PartitionKey { get; set; }

        void SetPartititionKeyProperty<T>(Expression<Func<T, object>> expression);

        object GetPartititionKeyValue();
    }
}