using ELODIE.Common;
using ELODIE.Entities;
using System;

namespace ELODIE.Rpc.product_type
{
    public class ProductType_ProductTypeDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long StatusId { get; set; }
        public bool Used { get; set; }
        public DateTime UpdateTime { get; set; }
        public ProductType_StatusDTO Status { get; set; }
        public ProductType_ProductTypeDTO() { }
        public ProductType_ProductTypeDTO(ProductType ProductType)
        {
            this.Id = ProductType.Id;
            this.Code = ProductType.Code;
            this.Name = ProductType.Name;
            this.Description = ProductType.Description;
            this.StatusId = ProductType.StatusId;
            this.Used = ProductType.Used;
            this.UpdateTime = ProductType.UpdatedAt;
            this.Status = ProductType.Status == null ? null : new ProductType_StatusDTO(ProductType.Status);
            this.Errors = ProductType.Errors;
        }
    }

    public class ProductType_ProductTypeFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Description { get; set; }
        public IdFilter StatusId { get; set; }
        public DateFilter UpdatedTime { get; set; }
        public ProductTypeOrder OrderBy { get; set; }
        public string Search { get; set; }

    }
}
