using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.CosmosDB;
using Volo.Abp.Data;
using Volo.Abp.TestApp.Domain;
using Xunit;

namespace Volo.Abp.CosmosDB.Domain
{
    public class ExtraProperties_Tests : CosmosDBTestBase
    {
        protected readonly ICityRepository CityRepository;

        public ExtraProperties_Tests()
        {
            CityRepository = GetRequiredService<ICityRepository>();
        }

        [Fact]
        public async Task Should_Get_An_Extra_Property()
        {
            var london = await CityRepository.FindByNameAsync("London").ConfigureAwait(false);
            london.HasProperty("population").ShouldBeTrue();
            london.GetProperty<int>("population").ShouldBe(10_470_000);
        }

        [Fact]
        public async Task Should_Add_An_Extra_Property()
        {
            var london = await CityRepository.FindByNameAsync("London").ConfigureAwait(false);
            london.SetProperty("areaAsKm", 1572);
            await CityRepository.UpdateAsync(london).ConfigureAwait(false);

            var london2 = await CityRepository.FindByNameAsync("London").ConfigureAwait(false);
            london2.HasProperty("areaAsKm").ShouldBeTrue();
            london2.GetProperty<int>("areaAsKm").ShouldBe(1572);
        }

        [Fact]
        public async Task Should_Update_An_Existing_Extra_Property()
        {
            var london = await CityRepository.FindByNameAsync("London").ConfigureAwait(false);

            london.ExtraProperties["Population"] = 11_000_042;
            await CityRepository.UpdateAsync(london).ConfigureAwait(false);

            var london2 = await CityRepository.FindByNameAsync("London").ConfigureAwait(false);
            london2.HasProperty("population").ShouldBeTrue();
            london2.GetProperty<int>("population").ShouldBe(11_000_042);
        }
    }
}