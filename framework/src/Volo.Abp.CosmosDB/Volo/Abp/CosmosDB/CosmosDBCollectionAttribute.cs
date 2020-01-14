using System;

namespace Volo.Abp.CosmosDB
{
    public class CosmosDBCollectionAttribute : Attribute
    {
        public string CollectionName { get; set; }

        public CosmosDBCollectionAttribute()
        {

        }

        public CosmosDBCollectionAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}