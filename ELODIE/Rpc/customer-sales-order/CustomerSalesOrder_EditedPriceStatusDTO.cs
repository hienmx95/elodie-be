using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.customer_sales_order
{
    public class CustomerSalesOrder_EditedPriceStatusDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public CustomerSalesOrder_EditedPriceStatusDTO() {}
        public CustomerSalesOrder_EditedPriceStatusDTO(EditedPriceStatus EditedPriceStatus)
        {
            
            this.Id = EditedPriceStatus.Id;
            
            this.Code = EditedPriceStatus.Code;
            
            this.Name = EditedPriceStatus.Name;
            
            this.Errors = EditedPriceStatus.Errors;
        }
    }

    public class CustomerSalesOrder_EditedPriceStatusFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public EditedPriceStatusOrder OrderBy { get; set; }
    }
}