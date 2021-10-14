using System;
using System.Collections.Generic;
using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ELODIE.Entities
{
    public class CustomerSalesOrderPaymentHistory : DataEntity,  IEquatable<CustomerSalesOrderPaymentHistory>
    {
        public long Id { get; set; }
        public long CustomerSalesOrderId { get; set; }
        public string PaymentMilestone { get; set; }
        public decimal? PaymentPercentage { get; set; }
        public decimal? PaymentAmount { get; set; }
        public long? PaymentTypeId { get; set; }
        public string Description { get; set; }
        public bool? IsPaid { get; set; }
        public CustomerSalesOrder CustomerSalesOrder { get; set; }
        public PaymentType PaymentType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(CustomerSalesOrderPaymentHistory other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.CustomerSalesOrderId != other.CustomerSalesOrderId) return false;
            if (this.PaymentMilestone != other.PaymentMilestone) return false;
            if (this.PaymentPercentage != other.PaymentPercentage) return false;
            if (this.PaymentAmount != other.PaymentAmount) return false;
            if (this.PaymentTypeId != other.PaymentTypeId) return false;
            if (this.Description != other.Description) return false;
            if (this.IsPaid != other.IsPaid) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class CustomerSalesOrderPaymentHistoryFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter CustomerSalesOrderId { get; set; }
        public StringFilter PaymentMilestone { get; set; }
        public DecimalFilter PaymentPercentage { get; set; }
        public DecimalFilter PaymentAmount { get; set; }
        public IdFilter PaymentTypeId { get; set; }
        public StringFilter Description { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<CustomerSalesOrderPaymentHistoryFilter> OrFilter { get; set; }
        public CustomerSalesOrderPaymentHistoryOrder OrderBy {get; set;}
        public CustomerSalesOrderPaymentHistorySelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CustomerSalesOrderPaymentHistoryOrder
    {
        Id = 0,
        CustomerSalesOrder = 1,
        PaymentMilestone = 2,
        PaymentPercentage = 3,
        PaymentAmount = 4,
        PaymentType = 5,
        Description = 6,
        IsPaid = 7,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum CustomerSalesOrderPaymentHistorySelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        CustomerSalesOrder = E._1,
        PaymentMilestone = E._2,
        PaymentPercentage = E._3,
        PaymentAmount = E._4,
        PaymentType = E._5,
        Description = E._6,
        IsPaid = E._7,
    }
}
