using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.customer
{
    public class Customer_SexDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public Customer_SexDTO() {}
        public Customer_SexDTO(Sex Sex)
        {
            
            this.Id = Sex.Id;
            
            this.Code = Sex.Code;
            
            this.Name = Sex.Name;
            
            this.Errors = Sex.Errors;
        }
    }

    public class Customer_SexFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public SexOrder OrderBy { get; set; }
    }
}