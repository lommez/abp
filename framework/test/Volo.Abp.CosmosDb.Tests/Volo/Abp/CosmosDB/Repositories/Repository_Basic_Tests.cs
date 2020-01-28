//using Shouldly;
//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Volo.Abp.TestApp;
//using Volo.Abp.TestApp.Domain;
//using Volo.Abp.TestApp.Testing;
//using Xunit;

//namespace Volo.Abp.CosmosDB.Repositories
//{
//    public class Repository_Basic_Tests : Repository_Basic_Tests<AbpCosmosDBTestModule>
//    {
//        [Fact]
//        public async Task Linq_Queries()
//        {
//            await WithUnitOfWorkAsync(() =>
//            {
//                PersonRepository.FirstOrDefault(p => p.Name == "Douglas").ShouldNotBeNull();
//                PersonRepository.Count().ShouldBeGreaterThan(0);
//                return Task.CompletedTask;
//            }).ConfigureAwait(false);
//        }

//        [Fact]
//        public async Task UpdateAsync()
//        {
//            var person = await PersonRepository.GetAsync(TestDataBuilder.UserDouglasId).ConfigureAwait(false);

//            person.ChangeName("Douglas-Updated");
//            person.Phones.Add(new Phone(person.Id, "6667778899", PhoneType.Office));

//            await PersonRepository.UpdateAsync(person).ConfigureAwait(false);

//            person = await PersonRepository.FindAsync(TestDataBuilder.UserDouglasId).ConfigureAwait(false);
//            person.ShouldNotBeNull();
//            person.Name.ShouldBe("Douglas-Updated");
//            person.Phones.Count.ShouldBe(3);
//            person.Phones.Any(p => p.PersonId == person.Id && p.Number == "6667778899" && p.Type == PhoneType.Office).ShouldBeTrue();
//        }

//        [Fact]
//        public override async Task InsertAsync()
//        {
//            var person = new Person(Guid.NewGuid(), "New Person", 35);
//            person.Phones.Add(new Phone(person.Id, "1234567890"));

//            await PersonRepository.InsertAsync(person).ConfigureAwait(false);

//            person = await PersonRepository.FindAsync(person.Id).ConfigureAwait(false);
//            person.ShouldNotBeNull();
//            person.Name.ShouldBe("New Person");
//            person.Phones.Count.ShouldBe(1);
//            person.Phones.Any(p => p.PersonId == person.Id && p.Number == "1234567890").ShouldBeTrue();
//        }
//    }
//}