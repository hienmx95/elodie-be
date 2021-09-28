using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class SupplierCategoryMappingDAO
    {
        public long SupplierId { get; set; }
        public long CategoryId { get; set; }

        public virtual CategoryDAO Category { get; set; }
        public virtual SupplierDAO Supplier { get; set; }
    }
}
