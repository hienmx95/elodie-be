using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace ELODIE.Entities
{
    public class Product : DataEntity, IEquatable<Product>
    {
        public long Id { get; set; }
        public Guid RowId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ScanCode { get; set; }
        public string ERPCode { get; set; }
        public long CategoryId { get; set; }
        public long ProductTypeId { get; set; }
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
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Brand Brand { get; set; }
        public CodeGeneratorRule CodeGeneratorRule { get; set; }
        public Category Category { get; set; }
        public ProductType ProductType { get; set; }
        public Status Status { get; set; }
        public TaxType TaxType { get; set; }
        public UsedVariation UsedVariation { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public UnitOfMeasureGrouping UnitOfMeasureGrouping { get; set; }
        public List<Item> Items { get; set; }
        public List<ProductImageMapping> ProductImageMappings { get; set; }
        public List<ProductProductGroupingMapping> ProductProductGroupingMappings { get; set; }
        public List<VariationGrouping> VariationGroupings { get; set; }

        public List<RequestHistory<Product>> ProductHistories { get; set; }

        public bool Equals(Product other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Code != other.Code) return false;
            if (this.Name != other.Name) return false;
            if (this.Description != other.Description) return false;
            if (this.ScanCode != other.ScanCode) return false;
            if (this.ERPCode != other.ERPCode) return false;
            if (this.CategoryId != other.CategoryId) return false;
            if (this.ProductTypeId != other.ProductTypeId) return false;
            if (this.BrandId != other.BrandId) return false;
            if (this.UnitOfMeasureId != other.UnitOfMeasureId) return false;
            if (this.CodeGeneratorRuleId != other.CodeGeneratorRuleId) return false;
            if (this.UnitOfMeasureGroupingId != other.UnitOfMeasureGroupingId) return false;
            if (this.SalePrice != other.SalePrice) return false;
            if (this.RetailPrice != other.RetailPrice) return false;
            if (this.TaxTypeId != other.TaxTypeId) return false;
            if (this.StatusId != other.StatusId) return false;
            if (this.OtherName != other.OtherName) return false;
            if (this.TechnicalName != other.TechnicalName) return false;
            if (this.Note != other.Note) return false;
            if (this.IsPurchasable != other.IsPurchasable) return false;
            if (this.IsSellable != other.IsSellable) return false;
            if (this.IsNew != other.IsNew) return false;
            if (this.UsedVariationId != other.UsedVariationId) return false;
            if (this.Used != other.Used) return false;
            if (this.RowId != other.RowId) return false;
            if (this.Items?.Count != other.Items?.Count) return false;
            else if (this.Items != null && other.Items != null)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    Item Item = Items[i];
                    Item otherItem = other.Items[i];
                    if (Item == null && otherItem != null)
                        return false;
                    if (Item != null && otherItem == null)
                        return false;
                    if (Item.Equals(otherItem) == false)
                        return false;
                }
            }
            if (this.ProductImageMappings?.Count != other.ProductImageMappings?.Count) return false;
            else if (this.ProductImageMappings != null && other.ProductImageMappings != null)
            {
                for (int i = 0; i < ProductImageMappings.Count; i++)
                {
                    ProductImageMapping ProductImageMapping = ProductImageMappings[i];
                    ProductImageMapping otherProductImageMapping = other.ProductImageMappings[i];
                    if (ProductImageMapping == null && otherProductImageMapping != null)
                        return false;
                    if (ProductImageMapping != null && otherProductImageMapping == null)
                        return false;
                    if (ProductImageMapping.Equals(otherProductImageMapping) == false)
                        return false;
                }
            }
            if (this.ProductProductGroupingMappings?.Count != other.ProductProductGroupingMappings?.Count) return false;
            else if (this.ProductProductGroupingMappings != null && other.ProductProductGroupingMappings != null)
            {
                for (int i = 0; i < ProductProductGroupingMappings.Count; i++)
                {
                    ProductProductGroupingMapping ProductProductGroupingMapping = ProductProductGroupingMappings[i];
                    ProductProductGroupingMapping otherProductProductGroupingMapping = other.ProductProductGroupingMappings[i];
                    if (ProductProductGroupingMapping == null && otherProductProductGroupingMapping != null)
                        return false;
                    if (ProductProductGroupingMapping != null && otherProductProductGroupingMapping == null)
                        return false;
                    if (ProductProductGroupingMapping.Equals(otherProductProductGroupingMapping) == false)
                        return false;
                }
            }
            if (this.VariationGroupings?.Count != other.VariationGroupings?.Count) return false;
            else if (this.VariationGroupings != null && other.VariationGroupings != null)
            {
                for (int i = 0; i < VariationGroupings.Count; i++)
                {
                    VariationGrouping VariationGrouping = VariationGroupings[i];
                    VariationGrouping otherVariationGrouping = other.VariationGroupings[i];
                    if (VariationGrouping == null && otherVariationGrouping != null)
                        return false;
                    if (VariationGrouping != null && otherVariationGrouping == null)
                        return false;
                    if (VariationGrouping.Equals(otherVariationGrouping) == false)
                        return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class ProductFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Description { get; set; }
        public StringFilter ScanCode { get; set; }
        public StringFilter ERPCode { get; set; }
        public IdFilter CategoryId { get; set; }
        public IdFilter ProductTypeId { get; set; }
        public IdFilter BrandId { get; set; }
        public IdFilter CodeGeneratorRuleId { get; set; }
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
        public bool? IsPurchasable { get; set; }
        public bool? IsSellable { get; set; }
        public string Search { get; set; }

        public List<ProductFilter> OrFilter { get; set; }
        public ProductOrder OrderBy { get; set; }
        public ProductSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProductOrder
    {
        Id = 0,
        Code = 1,
        Name = 3,
        Description = 4,
        ScanCode = 5,
        ProductType = 6,
        Brand = 8,
        UnitOfMeasure = 9,
        UnitOfMeasureGrouping = 10,
        SalePrice = 11,
        RetailPrice = 12,
        TaxType = 13,
        Status = 14,
        OtherName = 15,
        TechnicalName = 16,
        Note = 17,
        IsNew = 18,
        IsPurchasable = 19,
        IsSellable = 20,
        UsedVariation = 21,
        Category = 22,
        UpdatedAt = 23,
        CodeGeneratorRule = 24,
    }

    [Flags]
    public enum ProductSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._3,
        Description = E._4,
        ScanCode = E._5,
        ProductType = E._6,
        Brand = E._8,
        UnitOfMeasure = E._9,
        UnitOfMeasureGrouping = E._10,
        SalePrice = E._11,
        RetailPrice = E._12,
        TaxType = E._13,
        Status = E._14,
        OtherName = E._15,
        TechnicalName = E._16,
        Note = E._17,
        ERPCode = E._18,
        ProductProductGroupingMapping = E._19,
        IsNew = E._20,
        IsPurchasable = 21,
        IsSellable = 22,
        UsedVariation = E._23,
        VariationGrouping = E._24,
        Category = E._25,
        CodeGeneratorRule = E._26,
    }
}
