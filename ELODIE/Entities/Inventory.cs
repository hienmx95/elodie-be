using System;
using System.Collections.Generic;
using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ELODIE.Entities
{
    public class Inventory : DataEntity,  IEquatable<Inventory>
    {
        public long Id { get; set; }
        public long WarehouseId { get; set; }
        public long ItemId { get; set; }
        public long AlternateUnitOfMeasureId { get; set; }
        public decimal AlternateQuantity { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long Quantity { get; set; }
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
            if (this.Id != other.Id) return false;
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
        public IdFilter Id { get; set; }
        public IdFilter WarehouseId { get; set; }
        public IdFilter ItemId { get; set; }
        public IdFilter AlternateUnitOfMeasureId { get; set; }
        public DecimalFilter AlternateQuantity { get; set; }
        public IdFilter UnitOfMeasureId { get; set; }
        public LongFilter Quantity { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<InventoryFilter> OrFilter { get; set; }
        public InventoryOrder OrderBy {get; set;}
        public InventorySelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum InventoryOrder
    {
        Id = 0,
        Warehouse = 1,
        Item = 2,
        AlternateUnitOfMeasure = 3,
        AlternateQuantity = 4,
        UnitOfMeasure = 5,
        Quantity = 6,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum InventorySelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Warehouse = E._1,
        Item = E._2,
        AlternateUnitOfMeasure = E._3,
        AlternateQuantity = E._4,
        UnitOfMeasure = E._5,
        Quantity = E._6,
    }
}
