using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.product_grouping
{
    public class ProductGrouping_ProductGroupingDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long StatusId { get; set; }
        public long? ParentId { get; set; }
        public bool HasChildren { get; set; }
        public string Path { get; set; }
        public long Level { get; set; }
        public Guid RowId { get; set; }
        public ProductGrouping_ProductGroupingDTO Parent { get; set; }
        public ProductGrouping_StatusDTO Status { get; set; }
        public List<ProductGrouping_ProductProductGroupingMappingDTO> ProductProductGroupingMappings { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ProductGrouping_ProductGroupingDTO() { }
        public ProductGrouping_ProductGroupingDTO(ProductGrouping ProductGrouping)
        {
            this.Id = ProductGrouping.Id;
            this.Code = ProductGrouping.Code;
            this.Name = ProductGrouping.Name;
            this.Description = ProductGrouping.Description;
            this.StatusId = ProductGrouping.StatusId;
            this.ParentId = ProductGrouping.ParentId;
            this.HasChildren = ProductGrouping.HasChildren;
            this.Path = ProductGrouping.Path;
            this.Level = ProductGrouping.Level;
            this.RowId = ProductGrouping.RowId;
            this.Parent = ProductGrouping.Parent == null ? null : new ProductGrouping_ProductGroupingDTO(ProductGrouping.Parent);
            this.Status = ProductGrouping.Status == null ? null : new ProductGrouping_StatusDTO(ProductGrouping.Status);
            this.ProductProductGroupingMappings = ProductGrouping.ProductProductGroupingMappings?.Select(x => new ProductGrouping_ProductProductGroupingMappingDTO(x)).ToList();
            this.CreatedAt = ProductGrouping.CreatedAt;
            this.UpdatedAt = ProductGrouping.UpdatedAt;
            this.Errors = ProductGrouping.Errors;
        }
    }

    public class ProductGrouping_ProductGroupingFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Description { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter ParentId { get; set; }
        public StringFilter Path { get; set; }
        public LongFilter Level { get; set; }
        public GuidFilter RowId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public ProductGroupingOrder OrderBy { get; set; }
    }
}
