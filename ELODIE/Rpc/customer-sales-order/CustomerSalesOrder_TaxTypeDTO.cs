using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.customer_sales_order
{
    public class CustomerSalesOrder_TaxTypeDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public decimal Percentage { get; set; }
        
        public long StatusId { get; set; }
        
        public bool Used { get; set; }
        
        public Guid RowId { get; set; }
        

        public CustomerSalesOrder_TaxTypeDTO() {}
        public CustomerSalesOrder_TaxTypeDTO(TaxType TaxType)
        {
            
            this.Id = TaxType.Id;
            
            this.Code = TaxType.Code;
            
            this.Name = TaxType.Name;
            
            this.Percentage = TaxType.Percentage;
            
            this.StatusId = TaxType.StatusId;
            
            this.Used = TaxType.Used;
            
            this.RowId = TaxType.RowId;
            
            this.Errors = TaxType.Errors;
        }
    }

    public class CustomerSalesOrder_TaxTypeFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public DecimalFilter Percentage { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public GuidFilter RowId { get; set; }
        
        public TaxTypeOrder OrderBy { get; set; }
    }
}