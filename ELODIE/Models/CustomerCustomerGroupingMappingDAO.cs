using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class CustomerCustomerGroupingMappingDAO
    {
        public long CustomerId { get; set; }
        public long CustomerGroupingId { get; set; }

        public virtual CustomerDAO Customer { get; set; }
        public virtual CustomerGroupingDAO CustomerGrouping { get; set; }
    }
}
