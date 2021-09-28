using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class SupplierContactorDAO
    {
        public long Id { get; set; }
        public long SupplierId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool Used { get; set; }
        public Guid RowId { get; set; }

        public virtual SupplierDAO Supplier { get; set; }
    }
}
