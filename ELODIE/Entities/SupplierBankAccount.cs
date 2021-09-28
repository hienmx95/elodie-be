using System;
using System.Collections.Generic;
using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ELODIE.Entities
{
    public class SupplierBankAccount : DataEntity,  IEquatable<SupplierBankAccount>
    {
        public long Id { get; set; }
        public long SupplierId { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string BankAccountOwnerName { get; set; }
        public bool Used { get; set; }
        public Guid RowId { get; set; }
        public Supplier Supplier { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(SupplierBankAccount other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.SupplierId != other.SupplierId) return false;
            if (this.BankName != other.BankName) return false;
            if (this.BankAccount != other.BankAccount) return false;
            if (this.BankAccountOwnerName != other.BankAccountOwnerName) return false;
            if (this.Used != other.Used) return false;
            if (this.RowId != other.RowId) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class SupplierBankAccountFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter SupplierId { get; set; }
        public StringFilter BankName { get; set; }
        public StringFilter BankAccount { get; set; }
        public StringFilter BankAccountOwnerName { get; set; }
        public GuidFilter RowId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<SupplierBankAccountFilter> OrFilter { get; set; }
        public SupplierBankAccountOrder OrderBy {get; set;}
        public SupplierBankAccountSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SupplierBankAccountOrder
    {
        Id = 0,
        Supplier = 1,
        BankName = 2,
        BankAccount = 3,
        BankAccountOwnerName = 4,
        Used = 8,
        Row = 9,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum SupplierBankAccountSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Supplier = E._1,
        BankName = E._2,
        BankAccount = E._3,
        BankAccountOwnerName = E._4,
        Used = E._8,
        Row = E._9,
    }
}
