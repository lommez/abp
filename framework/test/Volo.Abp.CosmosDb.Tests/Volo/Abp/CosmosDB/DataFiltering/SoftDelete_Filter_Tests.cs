using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories.CosmosDB;
using Volo.Abp.TestApp;
using Volo.Abp.TestApp.Domain;
using Xunit;

namespace Volo.Abp.CosmosDB.DataFiltering
{
    public class SoftDelete_Filter_Tests : CosmosDBTestBase
    {
        protected readonly ICosmosDBRepository<Person, string> PersonRepository;
        protected readonly IDataFilter DataFilter;

        public SoftDelete_Filter_Tests()
        {
            PersonRepository = GetRequiredService<ICosmosDBRepository<Person, string>>();
            DataFilter = GetRequiredService<IDataFilter>();
        }

        [Fact]
        public async Task Should_Not_Get_Deleted_Entities_Linq()
        {
            var person = await PersonRepository.FirstOrDefaultAsync(p => p.Name == "John-Deleted");
            person.ShouldBeNull();
        }

        [Fact]
        public async Task Should_Not_Get_Deleted_Entities_By_Id()
        {
            var person = await PersonRepository.FindAsync(TestDataBuilder.UserJohnDeletedId.ToString(), TestDataBuilder.LastName).ConfigureAwait(false);
            person.ShouldBeNull();
        }

        [Fact]
        public async Task Should_Not_Get_Deleted_Entities_By_Default_ToList()
        {
            var people = await PersonRepository.GetListAsync();
            people.Count.ShouldBe(1);
            people.Any(p => p.Name == "Douglas").ShouldBeTrue();
        }

        [Fact]
        public async Task Should_Get_Deleted_Entities_When_Filter_Is_Disabled()
        {
            //Soft delete is enabled by default
            var people = await PersonRepository.GetListAsync();
            people.Any(p => !p.IsDeleted).ShouldBeTrue();
            people.Any(p => p.IsDeleted).ShouldBeFalse();

            using (DataFilter.Disable<ISoftDelete>())
            {
                //Soft delete is disabled
                people = await PersonRepository.GetListAsync();
                people.Any(p => !p.IsDeleted).ShouldBeTrue();
                people.Any(p => p.IsDeleted).ShouldBeTrue();

                using (DataFilter.Enable<ISoftDelete>())
                {
                    //Soft delete is enabled again
                    people = await PersonRepository.GetListAsync();
                    people.Any(p => !p.IsDeleted).ShouldBeTrue();
                    people.Any(p => p.IsDeleted).ShouldBeFalse();
                }

                //Soft delete is disabled (restored previous state)
                people = await PersonRepository.GetListAsync();
                people.Any(p => !p.IsDeleted).ShouldBeTrue();
                people.Any(p => p.IsDeleted).ShouldBeTrue();
            }

            //Soft delete is enabled (restored previous state)
            people = await PersonRepository.GetListAsync();
            people.Any(p => !p.IsDeleted).ShouldBeTrue();
            people.Any(p => p.IsDeleted).ShouldBeFalse();
        }
    }
}