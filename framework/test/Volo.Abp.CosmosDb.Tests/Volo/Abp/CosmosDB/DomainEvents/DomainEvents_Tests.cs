using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.CosmosDB;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.TestApp.Domain;
using Xunit;

namespace Volo.Abp.CosmosDB.DomainEvents
{
    public class DomainEvents_Tests : CosmosDBTestBase
    {
        protected readonly ICosmosDBRepository<Person, string> PersonRepository;
        protected readonly ILocalEventBus LocalEventBus;
        protected readonly IDistributedEventBus DistributedEventBus;

        public DomainEvents_Tests()
        {
            PersonRepository = GetRequiredService<ICosmosDBRepository<Person, string>>();
            LocalEventBus = GetRequiredService<ILocalEventBus>();
            DistributedEventBus = GetRequiredService<IDistributedEventBus>();
        }

        [Fact]
        public async Task Should_Trigger_Domain_Events_For_Aggregate_Root()
        {
            //Arrange

            var isLocalEventTriggered = false;
            var isDistributedEventTriggered = false;

            LocalEventBus.Subscribe<PersonNameChangedEvent>(data =>
            {
                data.OldName.ShouldBe("Douglas");
                data.Person.Name.ShouldBe("Douglas-Changed");
                isLocalEventTriggered = true;
                return Task.CompletedTask;
            });

            DistributedEventBus.Subscribe<PersonNameChangedEto>(data =>
            {
                data.OldName.ShouldBe("Douglas");
                data.NewName.ShouldBe("Douglas-Changed");
                isDistributedEventTriggered = true;
                return Task.CompletedTask;
            });

            //Act

            var dougles = await PersonRepository.FirstOrDefaultAsync(b => b.Name == "Douglas");
            dougles.ChangeName("Douglas-Changed");
            await PersonRepository.UpdateAsync(dougles).ConfigureAwait(false);

            //Assert

            isLocalEventTriggered.ShouldBeTrue();
            isDistributedEventTriggered.ShouldBeTrue();
        }
    }
}