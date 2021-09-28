using System;
using System.Collections.Generic;
using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ELODIE.Entities
{
    public class SupplierCategoryMapping : DataEntity,  IEquatable<SupplierCategoryMapping>
    {
        public long SupplierId { get; set; }
        public long CategoryId { get; set; }
        public Category Category { get; set; }
        public Supplier Supplier { get; set; }
        
        public bool Equals(SupplierCategoryMapping other)
        {
            if (other == null) return false;
            if (this.SupplierId != other.SupplierId) return false;
            if (this.CategoryId != other.CategoryId) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class SupplierCategoryMappingFilter : FilterEntity
    {
        public IdFilter SupplierId { get; set; }
        public IdFilter CategoryId { get; set; }
        public List<SupplierCategoryMappingFilter> OrFilter { get; set; }
        public SupplierCategoryMappingOrder OrderBy {get; set;}
        public SupplierCategoryMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SupplierCategoryMappingOrder
    {
        Supplier = 0,
        Category = 1,
    }

    [Flags]
    public enum SupplierCategoryMappingSelect:long
    {
        ALL = E.ALL,
        Supplier = E._0,
        Category = E._1,
    }
}
