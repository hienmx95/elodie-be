using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.customer_sales_order
{
    public class CustomerSalesOrder_PaymentTypeDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public long StatusId { get; set; }
        
        public bool Used { get; set; }
        
        public Guid RowId { get; set; }
        

        public CustomerSalesOrder_PaymentTypeDTO() {}
        public CustomerSalesOrder_PaymentTypeDTO(PaymentType PaymentType)
        {
            
            this.Id = PaymentType.Id;
            
            this.Code = PaymentType.Code;
            
            this.Name = PaymentType.Name;
            
            this.StatusId = PaymentType.StatusId;
            
            this.Used = PaymentType.Used;
            
            this.RowId = PaymentType.RowId;
            
            this.Errors = PaymentType.Errors;
        }
    }

    public class CustomerSalesOrder_PaymentTypeFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public GuidFilter RowId { get; set; }
        
        public PaymentTypeOrder OrderBy { get; set; }
    }
}