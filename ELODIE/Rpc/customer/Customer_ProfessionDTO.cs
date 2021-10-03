using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.customer
{
    public class Customer_ProfessionDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public long StatusId { get; set; }
        
        public Guid RowId { get; set; }
        
        public bool Used { get; set; }
        

        public Customer_ProfessionDTO() {}
        public Customer_ProfessionDTO(Profession Profession)
        {
            
            this.Id = Profession.Id;
            
            this.Code = Profession.Code;
            
            this.Name = Profession.Name;
            
            this.StatusId = Profession.StatusId;
            
            this.RowId = Profession.RowId;
            
            this.Used = Profession.Used;
            
            this.Errors = Profession.Errors;
        }
    }

    public class Customer_ProfessionFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public GuidFilter RowId { get; set; }
        
        public ProfessionOrder OrderBy { get; set; }
    }
}