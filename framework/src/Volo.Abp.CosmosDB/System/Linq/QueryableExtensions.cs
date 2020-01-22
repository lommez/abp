using JetBrains.Annotations;
using System.Linq.Expressions;
using Volo.Abp;

namespace System.Linq
{
    /// <summary>
    /// Some useful extension methods for <see cref="IQueryable{T}"/>.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Filters a <see cref="IQueryable{T}"/> by given predicate if given condition is true.
        /// </summary>
        /// <param name="query">Queryable to apply filtering</param>
        /// <param name="condition">A boolean value</param>
        /// <param name="skip">Predicate to skip the query</param>
        /// <returns>Filtered or not filtered query based on <paramref name="condition"/></returns>
        public static IQueryable<T> SkipIf<T>([NotNull] this IQueryable<T> query, bool condition, int? skip)
        {
            Check.NotNull(query, nameof(query));

            return condition
                ? query.Skip(skip.Value)
                : query;
        }

        /// <summary>
        /// Filters a <see cref="IQueryable{T}"/> by given predicate if given condition is true.
        /// </summary>
        /// <param name="query">Queryable to apply filtering</param>
        /// <param name="condition">A boolean value</param>
        /// <param name="take">Predicate to take the query</param>
        /// <returns>Filtered or not filtered query based on <paramref name="condition"/></returns>
        public static IQueryable<T> TakeIf<T>([NotNull] this IQueryable<T> query, bool condition, int? take)
        {
            Check.NotNull(query, nameof(query));

            return condition
                ? query.Take(take.Value)
                : query;
        }

        /// <summary>
        /// Filters a <see cref="IQueryable{T}"/> by given predicate if given condition is true.
        /// </summary>
        /// <param name="query">Queryable to apply ordering</param>
        /// <param name="condition">A boolean value</param>
        /// <param name="orderExpression">Expression to order by the result</param>
        /// <param name="orderDescending">A boolean value to order by descending</param>
        /// <returns>Filtered or not filtered query based on <paramref name="condition"/></returns>
        public static IQueryable<T> OrderByIf<T>([NotNull] this IQueryable<T> query, bool condition, Expression<Func<T, object>> orderExpression, bool orderDescending = false)
        {
            Check.NotNull(query, nameof(query));

            if (!orderDescending)
            {
                return condition
                    ? query.OrderBy(orderExpression)
                    : query;
            }
            else
            {
                return condition
                    ? query.OrderByDescending(orderExpression)
                    : query;
            }
        }
    }
}