using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace ELODIE.Entities
{
    public class InventoryHistory : DataEntity, IEquatable<InventoryHistory>
    {
        public long Id { get; set; }
        public long InventoryId { get; set; }
        public long OldSaleStock { get; set; }
        public long SaleStock { get; set; }
        public long OldAccountingStock { get; set; }
        public long AccountingStock { get; set; }
        public long AppUserId { get; set; }
        public DateTime UpdateTime { get; set; }
        public AppUser AppUser { get; set; }
        public Inventory Inventory { get; set; }

        public bool Equals(InventoryHistory other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class InventoryHistoryFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter InventoryId { get; set; }
        public LongFilter OldSaleStock { get; set; }
        public LongFilter SaleStock { get; set; }
        public LongFilter OldAccountingStock { get; set; }
        public LongFilter AccountingStock { get; set; }
        public IdFilter AppUserId { get; set; }
        public DateFilter UpdateTime { get; set; }
        public List<InventoryHistoryFilter> OrFilter { get; set; }
        public InventoryHistoryOrder OrderBy { get; set; }
        public InventoryHistorySelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum InventoryHistoryOrder
    {
        Id = 0,
        Inventory = 1,
        OldSaleStock = 2,
        SaleStock = 3,
        OldAccountingStock = 4,
        AccountingStock = 5,
        AppUser = 6,
        UpdateTime = 7
    }

    [Flags]
    public enum InventoryHistorySelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Inventory = E._1,
        OldSaleStock = E._2,
        SaleStock = E._3,
        OldAccountingStock = E._4,
        AccountingStock = E._5,
        AppUser = E._6,
        UpdateTime = E._7
    }
}
