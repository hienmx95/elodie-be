using System;
using System.Collections.Generic;
using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ELODIE.Entities
{
    public class SupplierContactor : DataEntity,  IEquatable<SupplierContactor>
    {
        public long Id { get; set; }
        public long SupplierId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool Used { get; set; }
        public Guid RowId { get; set; }
        public Supplier Supplier { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(SupplierContactor other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.SupplierId != other.SupplierId) return false;
            if (this.Name != other.Name) return false;
            if (this.Phone != other.Phone) return false;
            if (this.Email != other.Email) return false;
            if (this.Used != other.Used) return false;
            if (this.RowId != other.RowId) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class SupplierContactorFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter SupplierId { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Phone { get; set; }
        public StringFilter Email { get; set; }
        public GuidFilter RowId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<SupplierContactorFilter> OrFilter { get; set; }
        public SupplierContactorOrder OrderBy {get; set;}
        public SupplierContactorSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SupplierContactorOrder
    {
        Id = 0,
        Supplier = 1,
        Name = 2,
        Phone = 3,
        Email = 4,
        Used = 8,
        Row = 9,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum SupplierContactorSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Supplier = E._1,
        Name = E._2,
        Phone = E._3,
        Email = E._4,
        Used = E._8,
        Row = E._9,
    }
}
