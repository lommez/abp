using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Volo.Abp.CosmosDB.Extensions
{
    public static class FeedIteratorExtensions
    {
        public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(
            this FeedIterator<T> iterator,
            [EnumeratorCancellation]CancellationToken cancellationToken = default)
        {
            while (iterator.HasMoreResults)
            {
                foreach (var item in await iterator.ReadNextAsync(cancellationToken))
                {
                    yield return item;
                }
            }
        }

        public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(
            this FeedIterator<T> iterator,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            while (iterator.HasMoreResults)
            {
                var page = await iterator.ReadNextAsync(cancellationToken);
                foreach (var item in page)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return item;
                }
            }
        }
    }
}