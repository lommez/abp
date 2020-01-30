using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.Domain.Repositories.CosmosDB;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.TestApp;
using Volo.Abp.TestApp.Domain;
using Xunit;

namespace Volo.Abp.CosmosDB.DomainEvents
{
    public class EntityChangeEvents_Tests : CosmosDBTestBase
    {
        protected readonly ICosmosDBRepository<Person, string> PersonRepository;
        protected ILocalEventBus LocalEventBus { get; }
        protected IDistributedEventBus DistributedEventBus { get; }

        public EntityChangeEvents_Tests()
        {
            PersonRepository = GetRequiredService<ICosmosDBRepository<Person, string>>();
            LocalEventBus = GetRequiredService<ILocalEventBus>();
            DistributedEventBus = GetRequiredService<IDistributedEventBus>();
        }

        [Fact]
        public async Task Complex_Event_Test()
        {
            var personName = Guid.NewGuid().ToString("N");

            var creatingEventTriggered = false;
            var createdEventTriggered = false;
            var createdEtoTriggered = false;

            LocalEventBus.Subscribe<EntityCreatingEventData<Person>>(data =>
            {
                creatingEventTriggered.ShouldBeFalse();
                createdEventTriggered.ShouldBeFalse();

                creatingEventTriggered = true;

                data.Entity.Name.ShouldBe(personName);

                    /* Want to change age from 15 to 18 */
                data.Entity.Age.ShouldBe(15);
                data.Entity.Age = 18;
                return Task.CompletedTask;
            });

            LocalEventBus.Subscribe<EntityCreatedEventData<Person>>(data =>
            {
                creatingEventTriggered.ShouldBeTrue();
                createdEventTriggered.ShouldBeFalse();

                createdEventTriggered = true;

                data.Entity.Age.ShouldBe(18);
                data.Entity.Name.ShouldBe(personName);

                return Task.CompletedTask;
            });

            DistributedEventBus.Subscribe<EntityCreatedEto<PersonEto>>(eto =>
            {
                eto.Entity.Name.ShouldBe(personName);

                createdEtoTriggered = true;

                return Task.CompletedTask;
            });

            await PersonRepository.InsertAsync(new Person(Guid.NewGuid().ToString(), personName, TestDataBuilder.LastName, 15)).ConfigureAwait(false);

            creatingEventTriggered.ShouldBeTrue();
            createdEventTriggered.ShouldBeTrue();
            createdEtoTriggered.ShouldBeTrue();
        }
    }
}