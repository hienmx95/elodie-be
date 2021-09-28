using ELODIE.Common;
using ELODIE.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ELODIE.Rpc.product_grouping
{
    public class ProductGrouping_ProductDTO : DataDTO
    {

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ScanCode { get; set; }
        public long ProductTypeId { get; set; }
        public long CategoryId { get; set; }
        public long? SupplierId { get; set; }
        public long? BrandId { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long? UnitOfMeasureGroupingId { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public long TaxTypeId { get; set; }
        public long StatusId { get; set; }
        public string OtherName { get; set; }
        public string TechnicalName { get; set; }
        public string Note { get; set; }
        public long UsedVariationId { get; set; }
        public ProductGrouping_UsedVariationDTO UsedVariation { get; set; }
        public ProductGrouping_ProductTypeDTO ProductType { get; set; }
        public ProductGrouping_CategoryDTO Category { get; set; }
        public ProductGrouping_BrandDTO Brand { get; set; }
        public ProductGrouping_UnitOfMeasureDTO UnitOfMeasure { get; set; }
        public ProductGrouping_UnitOfMeasureGroupingDTO UnitOfMeasureGrouping { get; set; }
        public ProductGrouping_TaxTypeDTO TaxType { get; set; }
        public ProductGrouping_StatusDTO Status { get; set; }
        public List<ProductGrouping_ProductProductGroupingMappingDTO> ProductProductGroupingMappings { get; set; }
        public List<ProductGrouping_ProductImageMappingDTO> ProductImageMappings { get; set; }

        public ProductGrouping_ProductDTO() { }
        public ProductGrouping_ProductDTO(Product Product)
        {
            this.Id = Product.Id;
            this.Code = Product.Code;
            this.Name = Product.Name;
            this.Description = Product.Description;
            this.ScanCode = Product.ScanCode;
            this.ProductTypeId = Product.ProductTypeId;
            this.BrandId = Product.BrandId;
            this.UnitOfMeasureId = Product.UnitOfMeasureId;
            this.UnitOfMeasureGroupingId = Product.UnitOfMeasureGroupingId;
            this.SalePrice = Product.SalePrice;
            this.RetailPrice = Product.RetailPrice;
            this.TaxTypeId = Product.TaxTypeId;
            this.StatusId = Product.StatusId;
            this.OtherName = Product.OtherName;
            this.TechnicalName = Product.TechnicalName;
            this.Note = Product.Note;
            this.UsedVariationId = Product.UsedVariationId;
            this.Brand = Product.Brand == null ? null : new ProductGrouping_BrandDTO(Product.Brand);
            this.UsedVariation = Product.UsedVariation == null ? null : new ProductGrouping_UsedVariationDTO(Product.UsedVariation);
            this.ProductType = Product.ProductType == null ? null : new ProductGrouping_ProductTypeDTO(Product.ProductType);
            this.Category = Product.Category == null ? null : new ProductGrouping_CategoryDTO(Product.Category);
            this.Status = Product.Status == null ? null : new ProductGrouping_StatusDTO(Product.Status);
            this.TaxType = Product.TaxType == null ? null : new ProductGrouping_TaxTypeDTO(Product.TaxType);
            this.UnitOfMeasure = Product.UnitOfMeasure == null ? null : new ProductGrouping_UnitOfMeasureDTO(Product.UnitOfMeasure);
            this.UnitOfMeasureGrouping = Product.UnitOfMeasureGrouping == null ? null : new ProductGrouping_UnitOfMeasureGroupingDTO(Product.UnitOfMeasureGrouping);
            this.ProductProductGroupingMappings = Product.ProductProductGroupingMappings?.Select(x => new ProductGrouping_ProductProductGroupingMappingDTO(x)).ToList();
            this.ProductImageMappings = Product.ProductImageMappings?.Select(x => new ProductGrouping_ProductImageMappingDTO(x)).ToList();
            this.Errors = Product.Errors;
        }
    }

    public class ProductGrouping_ProductFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter SupplierCode { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Description { get; set; }
        public StringFilter ScanCode { get; set; }
        public IdFilter ProductTypeId { get; set; }
        public IdFilter BrandId { get; set; }
        public IdFilter CategoryId { get; set; }
        public IdFilter UnitOfMeasureId { get; set; }
        public IdFilter UnitOfMeasureGroupingId { get; set; }
        public DecimalFilter SalePrice { get; set; }
        public DecimalFilter RetailPrice { get; set; }
        public IdFilter TaxTypeId { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter ProductGroupingId { get; set; }
        public StringFilter OtherName { get; set; }
        public StringFilter TechnicalName { get; set; }
        public StringFilter Note { get; set; }
        public ProductOrder OrderBy { get; set; }

        public string Search { get; set; }
    }
}