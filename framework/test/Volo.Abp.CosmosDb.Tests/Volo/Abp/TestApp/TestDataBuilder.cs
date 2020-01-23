using Microsoft.Azure.Cosmos;
using System;
using System.Net;
using System.Threading.Tasks;
using Volo.Abp.CosmosDB;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories.CosmosDB;
using Volo.Abp.TestApp.CosmosDB;
using Volo.Abp.TestApp.Domain;
using Volo.Abp.Uow;

namespace Volo.Abp.TestApp
{
    public class TestDataBuilder : ITransientDependency
    {
        public static Guid TenantId1 { get; } = new Guid("55687dce-595c-41b4-a024-2a5e991ac8f4");
        public static Guid TenantId2 { get; } = new Guid("f522d19f-5a86-4278-98fb-0577319c544a");
        public static Guid UserDouglasId { get; } = new Guid("1fcf46b2-28c3-48d0-8bac-fa53268a2775");
        public static Guid UserJohnDeletedId { get; } = new Guid("1e28ca9f-df84-4f39-83fe-f5450ecbf5d4");
        public static string State { get; } = "SomeState";
        public static string LastName { get; } = "SomeLastName";

        public static Guid IstanbulCityId { get; } = new Guid("4d734a0e-3e6b-4bad-bb43-ef8cf1b09633");
        public static Guid LondonCityId { get; } = new Guid("27237527-605e-4652-a2a5-68e0e512da36");

        private readonly ICosmosDBContextProvider<ITestAppCosmosDBContext> _cosmosDBContextProvider;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IBasicCosmosDBRepository<Person, string> _personRepository;
        private readonly ICityRepository _cityRepository;

        public TestDataBuilder(
            ICosmosDBContextProvider<ITestAppCosmosDBContext> cosmosDBContextProvider,
            IUnitOfWorkManager unitOfWorkManager,
            IBasicCosmosDBRepository<Person, string> personRepository,
            ICityRepository cityRepository)
        {
            _cosmosDBContextProvider = cosmosDBContextProvider;
            _unitOfWorkManager = unitOfWorkManager;
            _personRepository = personRepository;
            _cityRepository = cityRepository;
        }

        public async Task BuildAsync()
        {
            await CreateContainers().ConfigureAwait(false);
            await AddCities().ConfigureAwait(false);
            await AddPeople().ConfigureAwait(false);
        }

        private async Task CreateContainers()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                var context = _cosmosDBContextProvider.GetDbContext();
                var cityContainer = context.Database.GetContainer("City");
                if (cityContainer != null)
                {
                    try
                    {
                        await cityContainer.DeleteContainerAsync();
                    }
                    catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                    {                                                
                    }
                    
                    await context.Database.CreateContainerIfNotExistsAsync("City", "/state");
                }

                var personContainer = context.Database.GetContainer("Person");
                if (personContainer != null)
                {
                    try
                    {
                        await personContainer.DeleteContainerAsync();
                    }
                    catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                    {                        
                    }                    
                    await context.Database.CreateContainerIfNotExistsAsync("Person", "/lastName");
                }
            }
        }

        private async Task AddCities()
        {
            var istanbul = new City(IstanbulCityId.ToString(), "Istanbul", State);
            istanbul.Districts.Add(new District(istanbul.Id, "Bakirkoy", 1283999));
            istanbul.Districts.Add(new District(istanbul.Id, "Mecidiyeköy", 2222321));
            istanbul.Districts.Add(new District(istanbul.Id, "Uskudar", 726172));

            await _cityRepository.InsertAsync(new City(Guid.NewGuid().ToString(), "Tokyo", State)).ConfigureAwait(false);
            await _cityRepository.InsertAsync(new City(Guid.NewGuid().ToString(), "Madrid", State)).ConfigureAwait(false);
            await _cityRepository.InsertAsync(new City(LondonCityId.ToString(), "London", State) { ExtraProperties = { { "Population", 10_470_000 } } }).ConfigureAwait(false);
            await _cityRepository.InsertAsync(istanbul).ConfigureAwait(false);
            await _cityRepository.InsertAsync(new City(Guid.NewGuid().ToString(), "Paris", State)).ConfigureAwait(false);
            await _cityRepository.InsertAsync(new City(Guid.NewGuid().ToString(), "Washington", State)).ConfigureAwait(false);
            await _cityRepository.InsertAsync(new City(Guid.NewGuid().ToString(), "Sao Paulo", State)).ConfigureAwait(false);
            await _cityRepository.InsertAsync(new City(Guid.NewGuid().ToString(), "Berlin", State)).ConfigureAwait(false);
            await _cityRepository.InsertAsync(new City(Guid.NewGuid().ToString(), "Amsterdam", State)).ConfigureAwait(false);
            await _cityRepository.InsertAsync(new City(Guid.NewGuid().ToString(), "Beijing", State)).ConfigureAwait(false);
            await _cityRepository.InsertAsync(new City(Guid.NewGuid().ToString(), "Rome", State)).ConfigureAwait(false);
        }

        private async Task AddPeople()
        {
            var douglas = new Person(UserDouglasId.ToString(), "Douglas", LastName, 42, cityId: LondonCityId.ToString());
            douglas.Phones.Add(new Phone(douglas.Id, "123456789"));
            douglas.Phones.Add(new Phone(douglas.Id, "123456780", PhoneType.Home));

            await _personRepository.InsertAsync(douglas).ConfigureAwait(false);

            await _personRepository.InsertAsync(new Person(UserJohnDeletedId.ToString(), "John-Deleted", LastName, 33) { IsDeleted = true }).ConfigureAwait(false);

            var tenant1Person1 = new Person(Guid.NewGuid().ToString(), TenantId1 + "-Person1", LastName, 42, TenantId1);
            var tenant1Person2 = new Person(Guid.NewGuid().ToString(), TenantId1 + "-Person2", LastName, 43, TenantId1);

            await _personRepository.InsertAsync(tenant1Person1).ConfigureAwait(false);
            await _personRepository.InsertAsync(tenant1Person2).ConfigureAwait(false);
        }
    }
}