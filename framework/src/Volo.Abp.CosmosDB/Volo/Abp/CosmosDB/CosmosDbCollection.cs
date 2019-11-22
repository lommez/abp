using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;

namespace Volo.Abp.CosmosDB
{
    public class CosmosDbCollection<TDocument> : ICosmosDbCollection<TDocument>
        where TDocument: class, IEntity<string>
    {
    }
}
