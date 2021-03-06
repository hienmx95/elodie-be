using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class CustomerSourceDAO
    {
        public CustomerSourceDAO()
        {
            Customers = new HashSet<CustomerDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool Used { get; set; }
        public Guid RowId { get; set; }

        public virtual ICollection<CustomerDAO> Customers { get; set; }
    }
}
