using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abp.TestApp.Domain
{
    public class City : AggregateRoot<string>, ICosmosDBEntity<string>
    {
        public string Name { get; set; }

        public ICollection<District> Districts { get; set; }

        public string State { get; set; }

        [JsonIgnore]
        public string PartitionKeyValue => State;

        [JsonProperty("_rid")]
        public string _rid { get; protected set; }

        [JsonProperty("_self")]
        public string _self { get; protected set; }

        [JsonProperty("_etag")]
        public string _etag { get; protected set; }

        [JsonProperty("_attachments")]
        public string _attachments { get; protected set; }

        [JsonProperty("_ts")]
        public long _ts { get; protected set; }

        private City()
        {
        }

        public City(string id, string name, string state)
            : base(id)
        {
            Name = name;
            State = state;
            Districts = new List<District>();
        }

        public int GetPopulation()
        {
            return Districts.Select(d => d.Population).Sum();
        }
    }
}