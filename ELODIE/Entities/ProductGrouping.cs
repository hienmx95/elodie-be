using System;
using System.Collections.Generic;
using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ELODIE.Entities
{
    public class ProductGrouping : DataEntity, IEquatable<ProductGrouping>
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
        public ProductGrouping Parent { get; set; }
        public Status Status { get; set; }
        public List<ProductProductGroupingMapping> ProductProductGroupingMappings { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public bool Equals(ProductGrouping other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Code != other.Code) return false;
            if (this.Name != other.Name) return false;
            if (this.Description != other.Description) return false;
            if (this.StatusId != other.StatusId) return false;
            if (this.ParentId != other.ParentId) return false;
            if (this.HasChildren != other.HasChildren) return false;
            if (this.Path != other.Path) return false;
            if (this.Level != other.Level) return false;
            if (this.RowId != other.RowId) return false;
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
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ProductGroupingFilter : FilterEntity
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
        public List<ProductGroupingFilter> OrFilter { get; set; }
        public ProductGroupingOrder OrderBy { get; set; }
        public ProductGroupingSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProductGroupingOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
        Description = 3,
        Status = 4,
        Parent = 5,
        HasChildren = 6,
        Path = 7,
        Level = 8,
        Row = 12,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum ProductGroupingSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
        Description = E._3,
        Status = E._4,
        Parent = E._5,
        HasChildren = E._6,
        Path = E._7,
        Level = E._8,
        Row = E._12,
        ProductProductGroupingMapping = E._13,
    }
}
