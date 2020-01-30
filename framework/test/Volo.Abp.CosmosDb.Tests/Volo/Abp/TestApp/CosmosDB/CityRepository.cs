﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.CosmosDB;
using Volo.Abp.TestApp.Domain;

namespace Volo.Abp.TestApp.CosmosDB
{
    public class CityRepository : CosmosDBRepository<ITestAppCosmosDBContext, City, string>, ICityRepository
    {
        public CityRepository(ITestAppCosmosDBContext dbContext)
            : base(dbContext)
        {
        }

        public Task<City> FindByNameAsync(string name)
        {
            return FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<List<Person>> GetPeopleInTheCityAsync(string cityName)
        {
            var city = await FindByNameAsync(cityName).ConfigureAwait(false);
            var people = DbContext.People.GetQueryable().Where(p => p.CityId == city.Id).ToList();
            return people;
        }
    }
}