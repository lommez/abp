using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Volo.Abp.CosmosDB;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories.CosmosDB;
using Volo.Abp.Modularity;
using Volo.Abp.TestApp.Domain;
using Volo.Abp.Timing;
using Volo.Abp.Users;
using Xunit;

namespace Volo.Abp.TestApp.Testing
{
    public abstract class Auditing_Tests<TStartupModule> : CosmosDBTestBase
        where TStartupModule : IAbpModule
    {
        protected Guid? CurrentUserId;

        protected readonly ICosmosDBRepository<Person, string> PersonRepository;
        protected readonly IClock Clock;
        protected readonly IDataFilter DataFilter;

        protected Auditing_Tests()
        {
            PersonRepository = GetRequiredService<ICosmosDBRepository<Person, string>>();
            Clock = GetRequiredService<IClock>();
            DataFilter = GetRequiredService<IDataFilter>();
        }

        protected override void AfterAddApplication(IServiceCollection services)
        {
            var currentUser = Substitute.For<ICurrentUser>();
            currentUser.Id.Returns(ci => CurrentUserId);

            services.AddSingleton(currentUser);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("4b2790fc-3f51-43d5-88a1-a92d96a9e6ea")]
        public async Task Should_Set_Creation_Properties(string currentUserId)
        {
            if (currentUserId != null)
            {
                CurrentUserId = Guid.Parse(currentUserId);
            }

            var personId = Guid.NewGuid();

            await PersonRepository.InsertAsync(new Person(personId.ToString(), "Adam", "First", 42)).ConfigureAwait(false);

            var person = await PersonRepository.FindAsync(personId.ToString(), "First").ConfigureAwait(false);

            person.ShouldNotBeNull();
            person.CreationTime.ShouldBeLessThanOrEqualTo(Clock.Now);
            person.CreatorId.ShouldBe(CurrentUserId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("4b2790fc-3f51-43d5-88a1-a92d96a9e6ea")]
        public async Task Should_Set_Modification_Properties(string currentUserId)
        {
            if (currentUserId != null)
            {
                CurrentUserId = Guid.Parse(currentUserId);
            }

            var douglas = await PersonRepository.GetAsync(TestDataBuilder.UserDouglasId.ToString(), TestDataBuilder.LastName).ConfigureAwait(false);
            douglas.LastModificationTime.ShouldBeNull();

            douglas.Age++;

            await PersonRepository.UpdateAsync(douglas).ConfigureAwait(false);

            douglas = await PersonRepository.FindAsync(TestDataBuilder.UserDouglasId.ToString(), TestDataBuilder.LastName).ConfigureAwait(false);

            douglas.ShouldNotBeNull();
            douglas.LastModificationTime.ShouldNotBeNull();
            douglas.LastModificationTime.Value.ShouldBeLessThanOrEqualTo(Clock.Now);
            douglas.LastModifierId.ShouldBe(CurrentUserId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("4b2790fc-3f51-43d5-88a1-a92d96a9e6ea")]
        public async Task Should_Set_Deletion_Properties(string currentUserId)
        {
            if (currentUserId != null)
            {
                CurrentUserId = Guid.Parse(currentUserId);
            }

            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();

            sw1.Start();
            var list = await PersonRepository.GetListAsync();
            sw1.Stop();

            sw2.Start();
            var douglas = await PersonRepository.GetAsync(TestDataBuilder.UserDouglasId.ToString(), TestDataBuilder.LastName).ConfigureAwait(false);
            sw2.Stop();

            await PersonRepository.DeleteAsync(douglas).ConfigureAwait(false);

            douglas = await PersonRepository.FindAsync(TestDataBuilder.UserDouglasId.ToString(), TestDataBuilder.LastName).ConfigureAwait(false);

            douglas.ShouldBeNull();

            using (DataFilter.Disable<ISoftDelete>())
            {
                douglas = await PersonRepository.FindAsync(TestDataBuilder.UserDouglasId.ToString(), TestDataBuilder.LastName).ConfigureAwait(false);

                douglas.ShouldNotBeNull();
                douglas.DeletionTime.ShouldNotBeNull();
                douglas.DeletionTime.Value.ShouldBeLessThanOrEqualTo(Clock.Now);
                douglas.DeleterId.ShouldBe(CurrentUserId);
            }
        }
    }
}