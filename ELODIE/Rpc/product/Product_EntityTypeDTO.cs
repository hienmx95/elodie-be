using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.product
{
    public class Product_EntityTypeDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public Product_EntityTypeDTO() {}
        public Product_EntityTypeDTO(EntityType EntityType)
        {
            
            this.Id = EntityType.Id;
            
            this.Code = EntityType.Code;
            
            this.Name = EntityType.Name;
            
            this.Errors = EntityType.Errors;
        }
    }

    public class CodeGeneratorRule_EntityTypeFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public EntityTypeOrder OrderBy { get; set; }
    }
}