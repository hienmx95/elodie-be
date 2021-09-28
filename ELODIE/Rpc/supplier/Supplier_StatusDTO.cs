using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.supplier
{
    public class Supplier_StatusDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public Supplier_StatusDTO() {}
        public Supplier_StatusDTO(Status Status)
        {
            
            this.Id = Status.Id;
            
            this.Code = Status.Code;
            
            this.Name = Status.Name;
            
            this.Errors = Status.Errors;
        }
    }

    public class Supplier_StatusFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public StatusOrder OrderBy { get; set; }
    }
}