using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.customer_sales_order
{
    public class CustomerSalesOrder_WardDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public long? Priority { get; set; }
        
        public long DistrictId { get; set; }
        
        public long StatusId { get; set; }
        
        public Guid RowId { get; set; }
        
        public bool Used { get; set; }
        

        public CustomerSalesOrder_WardDTO() {}
        public CustomerSalesOrder_WardDTO(Ward Ward)
        {
            
            this.Id = Ward.Id;
            
            this.Code = Ward.Code;
            
            this.Name = Ward.Name;
            
            this.Priority = Ward.Priority;
            
            this.DistrictId = Ward.DistrictId;
            
            this.StatusId = Ward.StatusId;
            
            this.RowId = Ward.RowId;
            
            this.Used = Ward.Used;
            
            this.Errors = Ward.Errors;
        }
    }

    public class CustomerSalesOrder_WardFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public LongFilter Priority { get; set; }
        
        public IdFilter DistrictId { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public GuidFilter RowId { get; set; }
        
        public WardOrder OrderBy { get; set; }
    }
}