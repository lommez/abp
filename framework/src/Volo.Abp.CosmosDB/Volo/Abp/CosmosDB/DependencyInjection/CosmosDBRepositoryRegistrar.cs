using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
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
            return typeof(CosmosDBRepository<,>).MakeGenericType(dbContextType, entityType);
        }

        protected override Type GetRepositoryType(Type dbContextType, Type entityType, Type primaryKeyType)
        {
            return typeof(CosmosDBRepository<,>).MakeGenericType(dbContextType, entityType);
        }

        public void AddCosmosDbRepositories()
        {
            foreach (var entityType in GetEntityTypes(Options.OriginalDbContextType))
            {
                RegisterCosmosDBRepository(entityType, GetDefaultRepositoryImplementationType(entityType));
            }
        }

        public void RegisterCosmosDBRepository(Type entityType, Type repositoryImplementationType)
        {
            //IReadOnlyBasicRepository<TEntity>
            var readOnlyBasicRepositoryInterface = typeof(IReadOnlyBasicCosmosDBRepository<>).MakeGenericType(entityType);
            if (readOnlyBasicRepositoryInterface.IsAssignableFrom(repositoryImplementationType))
            {
                Options.Services.TryAddTransient(readOnlyBasicRepositoryInterface, repositoryImplementationType);

                //IReadOnlyRepository<TEntity>
                var readOnlyRepositoryInterface = typeof(IReadOnlyCosmosDBRepository<>).MakeGenericType(entityType);
                if (readOnlyRepositoryInterface.IsAssignableFrom(repositoryImplementationType))
                {
                    Options.Services.TryAddTransient(readOnlyRepositoryInterface, repositoryImplementationType);
                }

                //IBasicRepository<TEntity>
                var basicRepositoryInterface = typeof(IBasicCosmosDBRepository<>).MakeGenericType(entityType);
                if (basicRepositoryInterface.IsAssignableFrom(repositoryImplementationType))
                {
                    Options.Services.TryAddTransient(basicRepositoryInterface, repositoryImplementationType);

                    //IRepository<TEntity>
                    var repositoryInterface = typeof(ICosmosDBRepository<>).MakeGenericType(entityType);
                    if (repositoryInterface.IsAssignableFrom(repositoryImplementationType))
                    {
                        Options.Services.TryAddTransient(repositoryInterface, repositoryImplementationType);
                    }
                }
            }
        }
    }
}