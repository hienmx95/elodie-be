using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.product_grouping
{
    public class ProductGrouping_UsedVariationDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public ProductGrouping_UsedVariationDTO() {}
        public ProductGrouping_UsedVariationDTO(UsedVariation UsedVariation)
        {
            
            this.Id = UsedVariation.Id;
            
            this.Code = UsedVariation.Code;
            
            this.Name = UsedVariation.Name;
            
            this.Errors = UsedVariation.Errors;
        }
    }
}