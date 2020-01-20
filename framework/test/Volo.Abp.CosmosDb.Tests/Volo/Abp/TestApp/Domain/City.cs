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

        public string PartitionKeyValue => State;

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