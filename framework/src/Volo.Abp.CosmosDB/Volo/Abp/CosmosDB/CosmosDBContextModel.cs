using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abp.CosmosDB
{
    public class CosmosDBContextModel
    {
        public IReadOnlyDictionary<Type, ICosmosDBEntityModel> Entities { get; }

        public CosmosDBContextModel(IReadOnlyDictionary<Type, ICosmosDBEntityModel> entities)
        {
            Entities = entities;
        }
    }
}