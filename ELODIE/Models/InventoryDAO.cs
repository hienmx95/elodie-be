using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class InventoryDAO
    {
        public long WarehouseId { get; set; }
        public long ItemId { get; set; }
        public long AlternateUnitOfMeasureId { get; set; }
        public decimal AlternateQuantity { get; set; }
        public long UnitOfMeasureId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual UnitOfMeasureDAO AlternateUnitOfMeasure { get; set; }
        public virtual ItemDAO Item { get; set; }
        public virtual UnitOfMeasureDAO UnitOfMeasure { get; set; }
        public virtual WarehouseDAO Warehouse { get; set; }
    }
}
