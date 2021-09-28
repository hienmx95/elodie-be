using ELODIE.Common;
using ELODIE.Entities;
using System;

namespace ELODIE.Rpc.brand
{
    public class Brand_BrandDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long StatusId { get; set; }
        public bool Used { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Brand_StatusDTO Status { get; set; }
        public Brand_BrandDTO() { }
        public Brand_BrandDTO(Brand Brand)
        {
            this.Id = Brand.Id;
            this.Code = Brand.Code;
            this.Name = Brand.Name;
            this.Description = Brand.Description;
            this.StatusId = Brand.StatusId;
            this.Used = Brand.Used;
            this.UpdatedAt = Brand.UpdatedAt;
            this.Status = Brand.Status == null ? null : new Brand_StatusDTO(Brand.Status);
            this.Errors = Brand.Errors;
        }
    }

    public class Brand_BrandFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Description { get; set; }
        public IdFilter StatusId { get; set; }
        public DateFilter UpdateTime { get; set; }
        public BrandOrder OrderBy { get; set; }
        public string Search { get; set; }
    }
}
