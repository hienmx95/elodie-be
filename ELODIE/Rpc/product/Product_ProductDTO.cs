using ELODIE.Common;
using ELODIE.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ELODIE.Rpc.product
{
    public class Product_ProductDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string ERPCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ScanCode { get; set; }
        public long CategoryId { get; set; }
        public long ProductTypeId { get; set; }
        public long? SupplierId { get; set; }
        public long? BrandId { get; set; }
        public long? CodeGeneratorRuleId { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long? UnitOfMeasureGroupingId { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public long TaxTypeId { get; set; }
        public long StatusId { get; set; }
        public string OtherName { get; set; }
        public string TechnicalName { get; set; }
        public string Note { get; set; }
        public bool IsNew { get; set; }
        public bool IsPurchasable { get; set; }
        public bool IsSellable { get; set; }
        public long UsedVariationId { get; set; }
        public long VariationCounter { get; set; }
        public bool Used { get; set; }
        public Product_BrandDTO Brand { get; set; }
        public Product_CodeGeneratorRuleDTO CodeGeneratorRule { get; set; }
        public Product_CategoryDTO Category { get; set; }
        public Product_ProductTypeDTO ProductType { get; set; }
        public Product_StatusDTO Status { get; set; }
        public Product_SupplierDTO Supplier { get; set; }
        public Product_TaxTypeDTO TaxType { get; set; }
        public Product_UnitOfMeasureDTO UnitOfMeasure { get; set; }
        public Product_UnitOfMeasureGroupingDTO UnitOfMeasureGrouping { get; set; }
        public Product_UsedVariationDTO UsedVariation { get; set; }
        public List<Product_ItemDTO> Items { get; set; }
        public List<Product_ProductImageMappingDTO> ProductImageMappings { get; set; }
        public List<Product_ProductProductGroupingMappingDTO> ProductProductGroupingMappings { get; set; }
        public List<Product_VariationGroupingDTO> VariationGroupings { get; set; }

        public List<Product_ProductHistoryDTO> ProductHistories { get; set; }

        public Product_ProductDTO() { }
        public Product_ProductDTO(Product Product)
        {
            this.Id = Product.Id;
            this.Code = Product.Code;
            this.ERPCode = Product.ERPCode;
            this.Name = Product.Name;
            this.Description = Product.Description;
            this.ScanCode = Product.ScanCode;
            this.CategoryId = Product.CategoryId;
            this.ProductTypeId = Product.ProductTypeId;
            this.BrandId = Product.BrandId;
            this.CodeGeneratorRuleId = Product.CodeGeneratorRuleId;
            this.UnitOfMeasureId = Product.UnitOfMeasureId;
            this.UnitOfMeasureGroupingId = Product.UnitOfMeasureGroupingId;
            this.SalePrice = Product.SalePrice;
            this.RetailPrice = Product.RetailPrice;
            this.TaxTypeId = Product.TaxTypeId;
            this.StatusId = Product.StatusId;
            this.OtherName = Product.OtherName;
            this.TechnicalName = Product.TechnicalName;
            this.Note = Product.Note;
            this.IsNew = Product.IsNew;
            this.IsPurchasable = Product.IsPurchasable;
            this.IsSellable = Product.IsSellable;
            this.UsedVariationId = Product.UsedVariationId;
            this.VariationCounter = Product.VariationCounter;
            this.Used = Product.Used;
            this.Brand = Product.Brand == null ? null : new Product_BrandDTO(Product.Brand);
            this.CodeGeneratorRule = Product.CodeGeneratorRule == null ? null : new Product_CodeGeneratorRuleDTO(Product.CodeGeneratorRule);
            this.Category  = Product.Category == null ? null : new Product_CategoryDTO(Product.Category);
            this.ProductType = Product.ProductType == null ? null : new Product_ProductTypeDTO(Product.ProductType);
            this.Status = Product.Status == null ? null : new Product_StatusDTO(Product.Status);
            this.TaxType = Product.TaxType == null ? null : new Product_TaxTypeDTO(Product.TaxType);
            this.UnitOfMeasure = Product.UnitOfMeasure == null ? null : new Product_UnitOfMeasureDTO(Product.UnitOfMeasure);
            this.UnitOfMeasureGrouping = Product.UnitOfMeasureGrouping == null ? null : new Product_UnitOfMeasureGroupingDTO(Product.UnitOfMeasureGrouping);
            this.UsedVariation = Product.UsedVariation == null ? null : new Product_UsedVariationDTO(Product.UsedVariation);
            this.Items = Product.Items?.Select(x => new Product_ItemDTO(x)).ToList();
            this.ProductImageMappings = Product.ProductImageMappings?.Select(x => new Product_ProductImageMappingDTO(x)).ToList();
            this.ProductProductGroupingMappings = Product.ProductProductGroupingMappings?.Select(x => new Product_ProductProductGroupingMappingDTO(x)).ToList();
            this.VariationGroupings = Product.VariationGroupings?.Select(x => new Product_VariationGroupingDTO(x)).ToList();

            this.ProductHistories = Product.ProductHistories?.Select(x => new Product_ProductHistoryDTO(x)).ToList();

            this.Errors = Product.Errors;
        }
    }

    public class Product_ProductFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter SupplierCode { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Description { get; set; }
        public StringFilter ScanCode { get; set; }
        public IdFilter CategoryId { get; set; }
        public IdFilter ProductTypeId { get; set; }
        public IdFilter SupplierId { get; set; }
        public IdFilter BrandId { get; set; }
        public IdFilter UnitOfMeasureId { get; set; }
        public IdFilter UnitOfMeasureGroupingId { get; set; }
        public DecimalFilter SalePrice { get; set; }
        public DecimalFilter RetailPrice { get; set; }
        public IdFilter TaxTypeId { get; set; }
        public IdFilter StatusId { get; set; }
        public StringFilter OtherName { get; set; }
        public StringFilter TechnicalName { get; set; }
        public StringFilter Note { get; set; }
        public IdFilter ProductGroupingId { get; set; }
        public IdFilter UsedVariationId { get; set; }
        public bool? IsNew { get; set; }
        public bool? IsPurchaseable { get; set; }
        public bool? IsSellable { get; set; }
        public string Search { get; set; }
        public ProductOrder OrderBy { get; set; }
    }
}
