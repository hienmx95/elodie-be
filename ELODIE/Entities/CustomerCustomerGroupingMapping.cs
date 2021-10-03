using System;
using System.Collections.Generic;
using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ELODIE.Entities
{
    public class CustomerCustomerGroupingMapping : DataEntity,  IEquatable<CustomerCustomerGroupingMapping>
    {
        public long CustomerId { get; set; }
        public long CustomerGroupingId { get; set; }
        public Customer Customer { get; set; }
        public CustomerGrouping CustomerGrouping { get; set; }
        
        public bool Equals(CustomerCustomerGroupingMapping other)
        {
            if (other == null) return false;
            if (this.CustomerId != other.CustomerId) return false;
            if (this.CustomerGroupingId != other.CustomerGroupingId) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class CustomerCustomerGroupingMappingFilter : FilterEntity
    {
        public IdFilter CustomerId { get; set; }
        public IdFilter CustomerGroupingId { get; set; }
        public List<CustomerCustomerGroupingMappingFilter> OrFilter { get; set; }
        public CustomerCustomerGroupingMappingOrder OrderBy {get; set;}
        public CustomerCustomerGroupingMappingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CustomerCustomerGroupingMappingOrder
    {
        Customer = 0,
        CustomerGrouping = 1,
    }

    [Flags]
    public enum CustomerCustomerGroupingMappingSelect:long
    {
        ALL = E.ALL,
        Customer = E._0,
        CustomerGrouping = E._1,
    }
}
