using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class CustomerGroupingDAO
    {
        public CustomerGroupingDAO()
        {
            CustomerCustomerGroupingMappings = new HashSet<CustomerCustomerGroupingMappingDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }
        public string Path { get; set; }
        public long Level { get; set; }
        public long StatusId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<CustomerCustomerGroupingMappingDAO> CustomerCustomerGroupingMappings { get; set; }
    }
}
