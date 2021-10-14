using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.customer_sales_order
{
    public class CustomerSalesOrder_OrderSourceDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public long? Priority { get; set; }
        
        public string Description { get; set; }
        
        public long StatusId { get; set; }
        
        public bool Used { get; set; }
        
        public Guid RowId { get; set; }
        

        public CustomerSalesOrder_OrderSourceDTO() {}
        public CustomerSalesOrder_OrderSourceDTO(OrderSource OrderSource)
        {
            
            this.Id = OrderSource.Id;
            
            this.Code = OrderSource.Code;
            
            this.Name = OrderSource.Name;
            
            this.Priority = OrderSource.Priority;
            
            this.Description = OrderSource.Description;
            
            this.StatusId = OrderSource.StatusId;
            
            this.Used = OrderSource.Used;
            
            this.RowId = OrderSource.RowId;
            
            this.Errors = OrderSource.Errors;
        }
    }

    public class CustomerSalesOrder_OrderSourceFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public LongFilter Priority { get; set; }
        
        public StringFilter Description { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public GuidFilter RowId { get; set; }
        
        public OrderSourceOrder OrderBy { get; set; }
    }
}