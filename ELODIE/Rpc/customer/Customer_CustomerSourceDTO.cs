using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.customer
{
    public class Customer_CustomerSourceDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public long StatusId { get; set; }
        
        public string Description { get; set; }
        
        public bool Used { get; set; }
        
        public Guid RowId { get; set; }
        

        public Customer_CustomerSourceDTO() {}
        public Customer_CustomerSourceDTO(CustomerSource CustomerSource)
        {
            
            this.Id = CustomerSource.Id;
            
            this.Code = CustomerSource.Code;
            
            this.Name = CustomerSource.Name;
            
            this.StatusId = CustomerSource.StatusId;
            
            this.Description = CustomerSource.Description;
            
            this.Used = CustomerSource.Used;
            
            this.RowId = CustomerSource.RowId;
            
            this.Errors = CustomerSource.Errors;
        }
    }

    public class Customer_CustomerSourceFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public StringFilter Description { get; set; }
        
        public GuidFilter RowId { get; set; }
        
        public CustomerSourceOrder OrderBy { get; set; }
    }
}