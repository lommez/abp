using Shouldly;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.CosmosDB;
using Volo.Abp.Specifications;
using Volo.Abp.TestApp.Domain;
using Xunit;

namespace Volo.Abp.CosmosDB.Repositories
{
    public class Repository_Specifications_Tests : CosmosDBTestBase
    {
        protected readonly ICosmosDBRepository<City, string> CityRepository;

        public Repository_Specifications_Tests()
        {
            CityRepository = GetRequiredService<ICosmosDBRepository<City, string>>();
        }

        [Fact]
        public async Task SpecificationWithRepository_Test()
        {
            var expression = new CitySpecification().ToExpression();
            var total = await CityRepository.GetCountAsync(expression);
            total.ShouldBe(1);
        }
    }

    public class CitySpecification : Specification<City>
    {
        public override Expression<Func<City, bool>> ToExpression()
        {
            return city => city.Name == "Istanbul";
        }
    }
}