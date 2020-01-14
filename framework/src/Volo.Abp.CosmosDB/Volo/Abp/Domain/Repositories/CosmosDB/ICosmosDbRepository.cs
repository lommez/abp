using JetBrains.Annotations;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abp.Domain.Repositories.CosmosDB
{
    public interface ICosmosDBRepository<TEntity> : IReadOnlyCosmosDBRepository<TEntity>, IBasicCosmosDBRepository<TEntity>
        where TEntity : class, ICosmosDBEntity
    {
        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        void Delete([NotNull] Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// Set true to automatically save changes to database.
        /// This is useful for ORMs / database APIs those only save changes with an explicit method call, but you need to immediately save changes to the database.
        /// </param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task DeleteAsync([NotNull] Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        //Task<Document> GetDocumentAsync(TEntity entity);

        //Task<IEnumerable<TEntity>> GetItemsAsync();

        //Task<IEnumerable<TEntity>> GetItemsAsync(Expression<Func<TEntity, bool>> predicate);

        //IEnumerable<TEntity> CreateDocumentQuery(string query, FeedOptions options);

        //Task<Document> CreateItemAsync(TEntity entity);

        //Task<Document> CreateItemAsync(TEntity entity, RequestOptions options);

        //Task<ResourceResponse<Attachment>> CreateAttachmentAsync(string attachmentsLink, object attachment, RequestOptions options);

        //Task<ResourceResponse<Attachment>> ReadAttachmentAsync(string attachmentLink, TEntity entity);

        //Task<ResourceResponse<Attachment>> ReplaceAttachmentAsync(Attachment attachment, RequestOptions options);

        //Task DeleteItemAsync(TEntity entity);

        //Task<StoredProcedureResponse<dynamic>> ExecuteStoredProcedureAsync(string procedureName, string query, TEntity entity);
    }
}