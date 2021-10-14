using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.customer_sales_order
{
    public class CustomerSalesOrder_OrderPaymentStatusDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public CustomerSalesOrder_OrderPaymentStatusDTO() {}
        public CustomerSalesOrder_OrderPaymentStatusDTO(OrderPaymentStatus OrderPaymentStatus)
        {
            
            this.Id = OrderPaymentStatus.Id;
            
            this.Code = OrderPaymentStatus.Code;
            
            this.Name = OrderPaymentStatus.Name;
            
            this.Errors = OrderPaymentStatus.Errors;
        }
    }

    public class CustomerSalesOrder_OrderPaymentStatusFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public OrderPaymentStatusOrder OrderBy { get; set; }
    }
}