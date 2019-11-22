using System;
using System.Collections.Generic;
using System.Text;

namespace Volo.Abp.CosmosDB
{
    public interface IAbpCosmosDbContext
    {
        IMongoDatabase Database { get; }

        IMongoCollection<T> Collection<T>();
    }
}
