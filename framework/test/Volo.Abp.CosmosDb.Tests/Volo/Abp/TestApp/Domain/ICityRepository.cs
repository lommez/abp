using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.CosmosDB;

namespace Volo.Abp.TestApp.Domain
{
    public interface ICityRepository : IBasicCosmosDBRepository<City, string>
    {
        Task<City> FindByNameAsync(string name);

        Task<List<Person>> GetPeopleInTheCityAsync(string cityName);
    }
}