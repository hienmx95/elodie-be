using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.product
{
    public class Product_EntityComponentDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public Product_EntityComponentDTO() {}
        public Product_EntityComponentDTO(EntityComponent EntityComponent)
        {
            
            this.Id = EntityComponent.Id;
            
            this.Code = EntityComponent.Code;
            
            this.Name = EntityComponent.Name;
            
            this.Errors = EntityComponent.Errors;
        }
    }

    public class CodeGeneratorRule_EntityComponentFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        public IdFilter EntityTypeId { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public EntityComponentOrder OrderBy { get; set; }
    }
}