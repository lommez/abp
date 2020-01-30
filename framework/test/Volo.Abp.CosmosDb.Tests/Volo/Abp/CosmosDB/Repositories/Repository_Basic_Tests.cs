using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.CosmosDB;
using Volo.Abp.TestApp;
using Volo.Abp.TestApp.Domain;
using Xunit;

namespace Volo.Abp.CosmosDB.Repositories
{
    public class Repository_Basic_Tests : CosmosDBTestBase
    {
        protected readonly ICosmosDBRepository<Person, string> PersonRepository;
        protected readonly ICityRepository CityRepository;

        public Repository_Basic_Tests()
        {
            PersonRepository = GetRequiredService<ICosmosDBRepository<Person, string>>();
            CityRepository = GetRequiredService<ICityRepository>();
        }

        [Fact]
        public async Task GetAsync()
        {
            var person = await PersonRepository.GetAsync(TestDataBuilder.UserDouglasId.ToString(), TestDataBuilder.LastName).ConfigureAwait(false);
            person.Name.ShouldBe("Douglas");
            person.Phones.Count.ShouldBe(2);
        }

        [Fact]
        public async Task FindAsync_Should_Return_Null_For_Not_Found_Entity()
        {
            var person = await PersonRepository.FindAsync(Guid.NewGuid().ToString()).ConfigureAwait(false);
            person.ShouldBeNull();
        }

        [Fact]
        public async Task DeleteAsync()
        {
            await PersonRepository.DeleteAsync(TestDataBuilder.UserDouglasId.ToString(), TestDataBuilder.LastName).ConfigureAwait(false);

            (await PersonRepository.FindAsync(TestDataBuilder.UserDouglasId.ToString(), TestDataBuilder.LastName).ConfigureAwait(false)).ShouldBeNull();
        }

        [Fact]
        public async Task Should_Access_To_Other_Collections_In_Same_Context_In_A_Custom_Method()
        {
            var people = await CityRepository.GetPeopleInTheCityAsync("London").ConfigureAwait(false);
            people.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task Custom_Repository_Method()
        {
            var city = await CityRepository.FindByNameAsync("Istanbul").ConfigureAwait(false);
            city.ShouldNotBeNull();
            city.Name.ShouldBe("Istanbul");
        }

        [Fact]
        public async Task InsertAsync()
        {
            var personId = Guid.NewGuid();

            await PersonRepository.InsertAsync(new Person(personId.ToString(), "Adam", TestDataBuilder.LastName, 42)).ConfigureAwait(false);

            var person = await PersonRepository.FindAsync(personId.ToString(), TestDataBuilder.LastName).ConfigureAwait(false);
            person.ShouldNotBeNull();
        }

        [Fact]
        public async Task InsertAsync2()
        {
            var person = new Person(Guid.NewGuid().ToString(), "New Person", TestDataBuilder.LastName, 35);
            person.Phones.Add(new Phone(person.Id, "1234567890"));

            await PersonRepository.InsertAsync(person).ConfigureAwait(false);

            person = await PersonRepository.FindAsync(person.Id).ConfigureAwait(false);
            person.ShouldNotBeNull();
            person.Name.ShouldBe("New Person");
            person.Phones.Count.ShouldBe(1);
            person.Phones.Any(p => p.PersonId == person.Id && p.Number == "1234567890").ShouldBeTrue();
        }

        [Fact]
        public async Task Linq_Queries()
        {
            var person = await PersonRepository.FirstOrDefaultAsync(p => p.Name == "Douglas");
            person.ShouldNotBeNull();
            var list = await PersonRepository.GetListAsync();
            list.Count().ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            var person = await PersonRepository.GetAsync(TestDataBuilder.UserDouglasId.ToString(), TestDataBuilder.LastName).ConfigureAwait(false);

            person.ChangeName("Douglas-Updated");
            person.Phones.Add(new Phone(person.Id, "6667778899", PhoneType.Office));

            await PersonRepository.UpdateAsync(person).ConfigureAwait(false);

            person = await PersonRepository.FindAsync(TestDataBuilder.UserDouglasId.ToString(), TestDataBuilder.LastName).ConfigureAwait(false);
            person.ShouldNotBeNull();
            person.Name.ShouldBe("Douglas-Updated");
            person.Phones.Count.ShouldBe(3);
            person.Phones.Any(p => p.PersonId == person.Id && p.Number == "6667778899" && p.Type == PhoneType.Office).ShouldBeTrue();
        }
    }
}