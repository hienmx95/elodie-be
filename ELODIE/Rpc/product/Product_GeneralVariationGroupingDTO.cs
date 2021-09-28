using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.product
{
    public class Product_GeneralVariationGroupingDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public bool Used { get; set; }
        public Guid RowId { get; set; }
        public Product_StatusDTO Status { get; set; }
        public List<Product_GeneralVariationDTO> GeneralVariations { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Product_GeneralVariationGroupingDTO() {}
        public Product_GeneralVariationGroupingDTO(GeneralVariationGrouping GeneralVariationGrouping)
        {
            this.Id = GeneralVariationGrouping.Id;
            this.Code = GeneralVariationGrouping.Code;
            this.Name = GeneralVariationGrouping.Name;
            this.StatusId = GeneralVariationGrouping.StatusId;
            this.Used = GeneralVariationGrouping.Used;
            this.RowId = GeneralVariationGrouping.RowId;
            this.Status = GeneralVariationGrouping.Status == null ? null : new Product_StatusDTO(GeneralVariationGrouping.Status);
            this.GeneralVariations = GeneralVariationGrouping.GeneralVariations?.Select(x => new Product_GeneralVariationDTO(x)).ToList();
            this.CreatedAt = GeneralVariationGrouping.CreatedAt;
            this.UpdatedAt = GeneralVariationGrouping.UpdatedAt;
            this.Errors = GeneralVariationGrouping.Errors;
        }
    }

    public class Product_GeneralVariationGroupingFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter StatusId { get; set; }
        public GuidFilter RowId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public GeneralVariationGroupingOrder OrderBy { get; set; }
    }
}
