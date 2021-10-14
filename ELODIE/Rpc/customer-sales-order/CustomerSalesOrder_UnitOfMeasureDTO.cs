using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.customer_sales_order
{
    public class CustomerSalesOrder_UnitOfMeasureDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public long StatusId { get; set; }
        
        public bool Used { get; set; }
        
        public Guid RowId { get; set; }
        public long? Factor { get; set; }

        public CustomerSalesOrder_UnitOfMeasureDTO() {}
        public CustomerSalesOrder_UnitOfMeasureDTO(UnitOfMeasure UnitOfMeasure)
        {
            
            this.Id = UnitOfMeasure.Id;
            
            this.Code = UnitOfMeasure.Code;
            
            this.Name = UnitOfMeasure.Name;
            
            this.Description = UnitOfMeasure.Description;
            
            this.StatusId = UnitOfMeasure.StatusId;
            
            this.Used = UnitOfMeasure.Used;
            
            this.RowId = UnitOfMeasure.RowId;
            
            this.Errors = UnitOfMeasure.Errors;
        }
        public CustomerSalesOrder_UnitOfMeasureDTO(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent)
        {

            this.Id = UnitOfMeasureGroupingContent.UnitOfMeasure == null ? 0 : UnitOfMeasureGroupingContent.UnitOfMeasure.Id;

            this.Code = UnitOfMeasureGroupingContent.UnitOfMeasure == null ? string.Empty : UnitOfMeasureGroupingContent.UnitOfMeasure.Code;

            this.Name = UnitOfMeasureGroupingContent.UnitOfMeasure == null ? string.Empty : UnitOfMeasureGroupingContent.UnitOfMeasure.Name;

            this.Description = UnitOfMeasureGroupingContent.UnitOfMeasure == null ? string.Empty : UnitOfMeasureGroupingContent.UnitOfMeasure.Description;

            this.StatusId = UnitOfMeasureGroupingContent.UnitOfMeasure == null ? 0 : UnitOfMeasureGroupingContent.UnitOfMeasure.StatusId;

            this.Factor = UnitOfMeasureGroupingContent.Factor;
        }
    }

    public class CustomerSalesOrder_UnitOfMeasureFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public StringFilter Description { get; set; }
        
        public IdFilter StatusId { get; set; }

        public IdFilter ProductId { get; set; }

        public GuidFilter RowId { get; set; }
        
        public UnitOfMeasureOrder OrderBy { get; set; }
    }
}