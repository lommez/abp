using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories.CosmosDB;
using Volo.Abp.TestApp;
using Volo.Abp.TestApp.Domain;
using Xunit;

namespace Volo.Abp.CosmosDB.DataFiltering
{
    public class SoftDelete_Tests : CosmosDBTestBase
    {
        protected readonly ICosmosDBRepository<Person, string> PersonRepository;
        protected readonly IDataFilter DataFilter;

        public SoftDelete_Tests()
        {
            PersonRepository = GetRequiredService<ICosmosDBRepository<Person, string>>();
            DataFilter = GetRequiredService<IDataFilter>();
        }

        [Fact]
        public async Task Should_Cancel_Deletion_For_Soft_Delete_Entities()
        {
            var douglas = await PersonRepository.GetAsync(TestDataBuilder.UserDouglasId.ToString(), TestDataBuilder.LastName).ConfigureAwait(false);
            await PersonRepository.DeleteAsync(douglas).ConfigureAwait(false);

            douglas = await PersonRepository.FindAsync(TestDataBuilder.UserDouglasId.ToString(), TestDataBuilder.LastName).ConfigureAwait(false);
            douglas.ShouldBeNull();

            using (DataFilter.Disable<ISoftDelete>())
            {
                douglas = await PersonRepository.FindAsync(TestDataBuilder.UserDouglasId.ToString(), TestDataBuilder.LastName).ConfigureAwait(false);
                douglas.ShouldNotBeNull();
                douglas.IsDeleted.ShouldBeTrue();
                douglas.DeletionTime.ShouldNotBeNull();
            }
        }

        [Fact]
        public async Task Should_Handle_Deletion_On_Update_For_Soft_Delete_Entities()
        {
            var douglas = await PersonRepository.GetAsync(TestDataBuilder.UserDouglasId.ToString(), TestDataBuilder.LastName).ConfigureAwait(false);
            douglas.Age = 42;
            douglas.IsDeleted = true;

            await PersonRepository.UpdateAsync(douglas).ConfigureAwait(false);

            douglas = await PersonRepository.FindAsync(TestDataBuilder.UserDouglasId.ToString(), TestDataBuilder.LastName).ConfigureAwait(false);
            douglas.ShouldBeNull();

            using (DataFilter.Disable<ISoftDelete>())
            {
                douglas = await PersonRepository.FindAsync(TestDataBuilder.UserDouglasId.ToString(), TestDataBuilder.LastName).ConfigureAwait(false);
                douglas.ShouldNotBeNull();
                douglas.IsDeleted.ShouldBeTrue();
                douglas.DeletionTime.ShouldNotBeNull();
                douglas.Age.ShouldBe(42);
            }
        }
    }
}