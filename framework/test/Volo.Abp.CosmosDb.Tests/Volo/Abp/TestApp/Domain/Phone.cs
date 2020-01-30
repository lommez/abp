using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.CosmosDB;

namespace Volo.Abp.TestApp.Domain
{
    [Table("AppPhones")]
    public class Phone : CosmosDBEntity<string>
    {
        public virtual string PersonId { get; set; }

        public virtual string Number { get; set; }

        public virtual PhoneType Type { get; set; }

        public override string PartitionKeyValue => Type.ToString();

        private Phone()
        {
        }

        public Phone(string personId, string number, PhoneType type = PhoneType.Mobile)
        {
            PersonId = personId;
            Number = number;
            Type = type;
        }

        public override object[] GetKeys()
        {
            return new object[] { PersonId, Number };
        }
    }

    public class Order : AggregateRoot<string>, ICosmosDBEntity<string>
    {
        public virtual string ReferenceNo { get; protected set; }

        public virtual float TotalItemCount { get; protected set; }

        public virtual DateTime CreationTime { get; protected set; }

        public virtual List<OrderLine> OrderLines { get; protected set; }

        [JsonIgnore]
        public string PartitionKeyValue => ReferenceNo;

        [JsonProperty("_rid")]
        public string _rid { get; protected set; }

        [JsonProperty("_self")]
        public string _self { get; protected set; }

        [JsonProperty("_etag")]
        public string _etag { get; protected set; }

        [JsonProperty("_attachments")]
        public string _attachments { get; protected set; }

        [JsonProperty("_ts")]
        public long _ts { get; protected set; }

        protected Order()
        {
        }

        public Order(string id, string referenceNo)
            : base(id)
        {
            ReferenceNo = referenceNo;
            OrderLines = new List<OrderLine>();
        }

        public void AddProduct(string productId, int count)
        {
            if (count <= 0)
            {
                throw new ArgumentException("You can not add zero or negative count of products!", nameof(count));
            }

            var existingLine = OrderLines.FirstOrDefault(ol => ol.ProductId == productId);

            if (existingLine == null)
            {
                OrderLines.Add(new OrderLine(this.Id, productId, count));
            }
            else
            {
                existingLine.ChangeCount(existingLine.Count + count);
            }

            TotalItemCount += count;
        }
    }

    public class OrderLine : CosmosDBEntity<string>
    {
        public virtual string OrderId { get; protected set; }

        public virtual string ProductId { get; protected set; }

        public virtual int Count { get; protected set; }

        public override string PartitionKeyValue => OrderId;

        protected OrderLine()
        {
        }

        public OrderLine(string orderId, string productId, int count)
        {
            OrderId = orderId;
            ProductId = productId;
            Count = count;
        }

        internal void ChangeCount(int newCount)
        {
            Count = newCount;
        }

        public override object[] GetKeys()
        {
            return new object[] { OrderId, ProductId };
        }
    }
}