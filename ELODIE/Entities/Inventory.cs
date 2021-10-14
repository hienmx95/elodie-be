using System;
using System.Collections.Generic;
using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ELODIE.Entities
{
    public class Inventory : DataEntity,  IEquatable<Inventory>
    {
        public long WarehouseId { get; set; }
        public long ItemId { get; set; }
        public long AlternateUnitOfMeasureId { get; set; }
        public decimal AlternateQuantity { get; set; }
        public long UnitOfMeasureId { get; set; }
        public decimal Quantity { get; set; }
        public UnitOfMeasure AlternateUnitOfMeasure { get; set; }
        public Item Item { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public Warehouse Warehouse { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(Inventory other)
        {
            if (other == null) return false;
            if (this.WarehouseId != other.WarehouseId) return false;
            if (this.ItemId != other.ItemId) return false;
            if (this.AlternateUnitOfMeasureId != other.AlternateUnitOfMeasureId) return false;
            if (this.AlternateQuantity != other.AlternateQuantity) return false;
            if (this.UnitOfMeasureId != other.UnitOfMeasureId) return false;
            if (this.Quantity != other.Quantity) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class InventoryFilter : FilterEntity
    {
        public IdFilter WarehouseId { get; set; }
        public IdFilter ItemId { get; set; }
        public IdFilter AlternateUnitOfMeasureId { get; set; }
        public DecimalFilter AlternateQuantity { get; set; }
        public IdFilter UnitOfMeasureId { get; set; }
        public DecimalFilter Quantity { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<InventoryFilter> OrFilter { get; set; }
        public InventoryOrder OrderBy {get; set;}
        public InventorySelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum InventoryOrder
    {
        Warehouse = 0,
        Item = 1,
        AlternateUnitOfMeasure = 2,
        AlternateQuantity = 3,
        UnitOfMeasure = 4,
        Quantity = 5,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum InventorySelect:long
    {
        ALL = E.ALL,
        Warehouse = E._0,
        Item = E._1,
        AlternateUnitOfMeasure = E._2,
        AlternateQuantity = E._3,
        UnitOfMeasure = E._4,
        Quantity = E._5,
    }
}
