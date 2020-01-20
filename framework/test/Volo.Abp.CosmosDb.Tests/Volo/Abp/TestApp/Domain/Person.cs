using System;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Domain.Entities.CosmosDB;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;

namespace Volo.Abp.TestApp.Domain
{
    public class Person : FullAuditedAggregateRoot<string>, ICosmosDBEntity<string>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        public virtual string? CityId { get; set; }

        public virtual string Name { get; private set; }

        public virtual string LastName { get; private set; }

        public virtual int Age { get; set; }

        public virtual DateTime? Birthday { get; set; }

        [DisableDateTimeNormalization]
        public virtual DateTime? LastActive { get; set; }

        public virtual Collection<Phone> Phones { get; set; }

        public string PartitionKeyValue => LastName;        

        private Person()
        {
        }

        public Person(string id, string name, string lastName, int age, Guid? tenantId = null, string? cityId = null)
            : base(id)
        {
            Name = name;
            LastName = lastName;
            Age = age;
            TenantId = tenantId;
            CityId = cityId;

            Phones = new Collection<Phone>();
        }

        public virtual void ChangeName(string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var oldName = Name;
            Name = name;

            AddLocalEvent(
                new PersonNameChangedEvent
                {
                    Person = this,
                    OldName = oldName
                }
            );

            AddDistributedEvent(
                new PersonNameChangedEto
                {
                    Id = Id,
                    OldName = oldName,
                    NewName = Name,
                    TenantId = TenantId
                }
            );
        }
    }
}