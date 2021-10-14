using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.warehouse
{
    public class Warehouse_InventoryDTO : DataDTO
    {
        public long WarehouseId { get; set; }
        public long ItemId { get; set; }
        public long AlternateUnitOfMeasureId { get; set; }
        public decimal AlternateQuantity { get; set; }
        public long UnitOfMeasureId { get; set; }
        public decimal Quantity { get; set; }
        public Warehouse_UnitOfMeasureDTO AlternateUnitOfMeasure { get; set; }   
        public Warehouse_ItemDTO Item { get; set; }   
        public Warehouse_UnitOfMeasureDTO UnitOfMeasure { get; set; }   

        public Warehouse_InventoryDTO() {}
        public Warehouse_InventoryDTO(Inventory Inventory)
        {
            this.WarehouseId = Inventory.WarehouseId;
            this.ItemId = Inventory.ItemId;
            this.AlternateUnitOfMeasureId = Inventory.AlternateUnitOfMeasureId;
            this.AlternateQuantity = Inventory.AlternateQuantity;
            this.UnitOfMeasureId = Inventory.UnitOfMeasureId;
            this.Quantity = Inventory.Quantity;
            this.AlternateUnitOfMeasure = Inventory.AlternateUnitOfMeasure == null ? null : new Warehouse_UnitOfMeasureDTO(Inventory.AlternateUnitOfMeasure);
            this.Item = Inventory.Item == null ? null : new Warehouse_ItemDTO(Inventory.Item);
            this.UnitOfMeasure = Inventory.UnitOfMeasure == null ? null : new Warehouse_UnitOfMeasureDTO(Inventory.UnitOfMeasure);
            this.Errors = Inventory.Errors;
        }
    }

    public class Warehouse_InventoryFilterDTO : FilterDTO
    {
        
        public IdFilter WarehouseId { get; set; }
        
        public IdFilter ItemId { get; set; }
        
        public IdFilter AlternateUnitOfMeasureId { get; set; }
        
        public DecimalFilter AlternateQuantity { get; set; }
        
        public IdFilter UnitOfMeasureId { get; set; }
        
        public DecimalFilter Quantity { get; set; }
        
        public InventoryOrder OrderBy { get; set; }
    }
}