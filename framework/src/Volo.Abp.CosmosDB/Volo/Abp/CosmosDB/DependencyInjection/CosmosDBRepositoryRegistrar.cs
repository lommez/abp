using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.CosmosDB;

namespace Volo.Abp.CosmosDB.DependencyInjection
{
    public class CosmosDBRepositoryRegistrar : RepositoryRegistrarBase<AbpCosmosDBContextRegistrationOptions>
    {
        public CosmosDBRepositoryRegistrar(AbpCosmosDBContextRegistrationOptions options)
            : base(options)
        {
        }

        protected override IEnumerable<Type> GetEntityTypes(Type dbContextType)
        {
            return CosmosDBContextHelper.GetEntityTypes(dbContextType);
        }

        protected override Type GetRepositoryType(Type dbContextType, Type entityType)
        {
            throw new NotSupportedException("This method is not supported for CosmosDB Repositories.");
        }

        protected override Type GetRepositoryType(Type dbContextType, Type entityType, Type primaryKeyType)
        {
            throw new NotSupportedException("This method is not supported for CosmosDB Repositories.");
        }

        public override void AddRepositories()
        {
            foreach (var entityType in GetEntityTypes(Options.OriginalDbContextType))
            {
                var partitionKeyType = entityType.BaseType.GenericTypeArguments[0];
                RegisterCosmosDBRepository(entityType, partitionKeyType, GetDefaultRepositoryImplementationType(entityType));
            }
        }

        protected virtual Type GetCosmosDBRepositoryType(Type dbContextType, Type entityType, Type partitionKeyType)
        {
            return typeof(CosmosDBRepository<,,>).MakeGenericType(dbContextType, entityType, partitionKeyType);
        }

        protected virtual Type GetCosmosDBRepositoryType(Type dbContextType, Type entityType, Type primaryKeyType, Type partitionKeyType)
        {
            return typeof(CosmosDBRepository<,,>).MakeGenericType(dbContextType, entityType, partitionKeyType);
        }

        protected override Type GetDefaultRepositoryImplementationType(Type entityType)
        {
            var primaryKeyType = EntityHelper.FindPrimaryKeyType(entityType);
            var partitionKeyType = entityType.BaseType.GenericTypeArguments[0];

            if (primaryKeyType == null)
            {
                return Options.SpecifiedDefaultRepositoryTypes
                    ? Options.DefaultRepositoryImplementationTypeWithoutKey.MakeGenericType(entityType)
                    : GetCosmosDBRepositoryType(Options.DefaultRepositoryDbContextType, entityType, partitionKeyType);
            }

            return Options.SpecifiedDefaultRepositoryTypes
                ? Options.DefaultRepositoryImplementationType.MakeGenericType(entityType, primaryKeyType)
                : GetCosmosDBRepositoryType(Options.DefaultRepositoryDbContextType, entityType, primaryKeyType, partitionKeyType);
        }

        protected virtual void RegisterCosmosDBRepository(Type entityType, Type partitionKeyType, Type repositoryImplementationType)
        {
            //IReadOnlyBasicRepository<TEntity>
            var readOnlyBasicRepositoryInterface = typeof(IReadOnlyBasicCosmosDBRepository<,>).MakeGenericType(entityType, partitionKeyType);
            if (readOnlyBasicRepositoryInterface.IsAssignableFrom(repositoryImplementationType))
            {
                Options.Services.TryAddTransient(readOnlyBasicRepositoryInterface, repositoryImplementationType);

                //IReadOnlyRepository<TEntity>
                var readOnlyRepositoryInterface = typeof(IReadOnlyCosmosDBRepository<,>).MakeGenericType(entityType, partitionKeyType);
                if (readOnlyRepositoryInterface.IsAssignableFrom(repositoryImplementationType))
                {
                    Options.Services.TryAddTransient(readOnlyRepositoryInterface, repositoryImplementationType);
                }

                //IBasicRepository<TEntity>
                var basicRepositoryInterface = typeof(IBasicCosmosDBRepository<,>).MakeGenericType(entityType, partitionKeyType);
                if (basicRepositoryInterface.IsAssignableFrom(repositoryImplementationType))
                {
                    Options.Services.TryAddTransient(basicRepositoryInterface, repositoryImplementationType);

                    //IRepository<TEntity>
                    var repositoryInterface = typeof(ICosmosDBRepository<,>).MakeGenericType(entityType, partitionKeyType);
                    if (repositoryInterface.IsAssignableFrom(repositoryImplementationType))
                    {
                        Options.Services.TryAddTransient(repositoryInterface, repositoryImplementationType);
                    }
                }
            }
        }
    }
}