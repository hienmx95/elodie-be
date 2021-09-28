using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.product
{
    public class Product_GeneralVariationDTO : DataDTO
    {
        public long Id { get; set; }
        public long GeneralVariationGroupingId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public bool Used { get; set; }
        public Guid RowId { get; set; }
        public Product_StatusDTO Status { get; set; }   

        public Product_GeneralVariationDTO() {}
        public Product_GeneralVariationDTO(GeneralVariation GeneralVariation)
        {
            this.Id = GeneralVariation.Id;
            this.GeneralVariationGroupingId = GeneralVariation.GeneralVariationGroupingId;
            this.Code = GeneralVariation.Code;
            this.Name = GeneralVariation.Name;
            this.StatusId = GeneralVariation.StatusId;
            this.Used = GeneralVariation.Used;
            this.RowId = GeneralVariation.RowId;
            this.Status = GeneralVariation.Status == null ? null : new Product_StatusDTO(GeneralVariation.Status);
            this.RowId = GeneralVariation.RowId;
            this.Errors = GeneralVariation.Errors;
        }
    }

    public class Product_GeneralVariationFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public IdFilter GeneralVariationGroupingId { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public GuidFilter RowId { get; set; }
        
        public GeneralVariationOrder OrderBy { get; set; }
    }
}