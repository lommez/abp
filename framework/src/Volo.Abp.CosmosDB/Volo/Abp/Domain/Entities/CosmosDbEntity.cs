using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Volo.Abp.Domain.Entities.CosmosDB
{
    public class CosmosDBEntity : Entity<string>, ICosmosDBEntity
    {
        private Expression<Func<CosmosDBEntity, object>> _expression;

        public string PartitionKey { get; set; }

        public void SetPartititionKeyProperty<T>(Expression<Func<T, object>> expression)
        {
            _expression = LambdaExpression.Lambda<Func<CosmosDBEntity, object>>(expression.Body, expression.Parameters);
            //_expression = expression.Compile();
        }

        public object GetPartititionKeyValue()
        {
            var prop = (PropertyInfo)((MemberExpression)_expression.Body).Member;
            return prop.GetValue(this);
        }
    }
}