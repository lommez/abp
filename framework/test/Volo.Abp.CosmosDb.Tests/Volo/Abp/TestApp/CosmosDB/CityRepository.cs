using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.CosmosDB;
using Volo.Abp.TestApp.Domain;

namespace Volo.Abp.TestApp.CosmosDB
{
    public class CityRepository : CosmosDBRepository<ITestAppCosmosDBContext, City, string>, ICityRepository
    {
        private readonly ICosmosDBRepository<Person, string> _personRepository;

        public CityRepository(ITestAppCosmosDBContext dbContext, ICosmosDBRepository<Person, string> personRepository)
            : base(dbContext)
        {
            _personRepository = personRepository;
        }

        public Task<City> FindByNameAsync(string name)
        {
            return FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<List<Person>> GetPeopleInTheCityAsync(string cityName)
        {
            var city = await FindByNameAsync(cityName).ConfigureAwait(false);
            var people = await _personRepository.GetListAsync(p => p.CityId == city.Id).ConfigureAwait(false);
            return people;
        }
    }
}