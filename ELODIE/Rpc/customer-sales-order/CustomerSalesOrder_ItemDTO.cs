using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.customer_sales_order
{
    public class CustomerSalesOrder_ItemDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public long ProductId { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public string ScanCode { get; set; }
        
        public decimal? SalePrice { get; set; }
        
        public decimal? RetailPrice { get; set; }
        
        public long StatusId { get; set; }
        
        public bool Used { get; set; }
        
        public Guid RowId { get; set; }
        public CustomerSalesOrder_ProductDTO Product { get; set; }

        public CustomerSalesOrder_ItemDTO() {}
        public CustomerSalesOrder_ItemDTO(Item Item)
        {
            
            this.Id = Item.Id;
            
            this.ProductId = Item.ProductId;
            
            this.Code = Item.Code;
            
            this.Name = Item.Name;
            
            this.ScanCode = Item.ScanCode;
            
            this.SalePrice = Item.SalePrice;
            
            this.RetailPrice = Item.RetailPrice;
            
            this.StatusId = Item.StatusId;
            
            this.Used = Item.Used;
            
            this.RowId = Item.RowId;
            this.Product = Item.Product == null ? null : new CustomerSalesOrder_ProductDTO(Item.Product);
            
            this.Errors = Item.Errors;
        }
    }

    public class CustomerSalesOrder_ItemFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter ProductId { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public StringFilter ScanCode { get; set; }
        
        public DecimalFilter SalePrice { get; set; }
        
        public DecimalFilter RetailPrice { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public GuidFilter RowId { get; set; }
        public IdFilter ProductGroupingId { get; set; }
        public IdFilter ProductTypeId { get; set; }
        public StringFilter OtherName { get; set; }
        public ItemOrder OrderBy { get; set; }
    }
}