using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.customer_sales_order
{
    public class CustomerSalesOrder_CodeGeneratorRuleDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public long EntityTypeId { get; set; }
        
        public long AutoNumberLenth { get; set; }
        
        public long StatusId { get; set; }
        
        public Guid RowId { get; set; }
        
        public bool Used { get; set; }
        

        public CustomerSalesOrder_CodeGeneratorRuleDTO() {}
        public CustomerSalesOrder_CodeGeneratorRuleDTO(CodeGeneratorRule CodeGeneratorRule)
        {
            
            this.Id = CodeGeneratorRule.Id;
            
            this.EntityTypeId = CodeGeneratorRule.EntityTypeId;
            
            this.AutoNumberLenth = CodeGeneratorRule.AutoNumberLenth;
            
            this.StatusId = CodeGeneratorRule.StatusId;
            
            this.RowId = CodeGeneratorRule.RowId;
            
            this.Used = CodeGeneratorRule.Used;
            
            this.Errors = CodeGeneratorRule.Errors;
        }
    }

    public class CustomerSalesOrder_CodeGeneratorRuleFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter EntityTypeId { get; set; }
        
        public LongFilter AutoNumberLenth { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public GuidFilter RowId { get; set; }
        
        public CodeGeneratorRuleOrder OrderBy { get; set; }
    }
}