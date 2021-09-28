using System;
using System.Collections.Generic;
using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ELODIE.Entities
{
    public class Category : DataEntity,  IEquatable<Category>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public string Description { get; set; }
        public long? ParentId { get; set; }
        public string Path { get; set; }
        public long Level { get; set; }
        public bool HasChildren { get; set; }
        public long StatusId { get; set; }
        public long? ImageId { get; set; }
        public Guid RowId { get; set; }
        public bool Used { get; set; }
        public Image Image { get; set; }
        public Category Parent { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(Category other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Code != other.Code) return false;
            if (this.Name != other.Name) return false;
            if (this.Prefix != other.Prefix) return false;
            if (this.Description != other.Description) return false;
            if (this.ParentId != other.ParentId) return false;
            if (this.Path != other.Path) return false;
            if (this.Level != other.Level) return false;
            if (this.HasChildren != other.HasChildren) return false;
            if (this.StatusId != other.StatusId) return false;
            if (this.ImageId != other.ImageId) return false;
            if (this.RowId != other.RowId) return false;
            if (this.Used != other.Used) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class CategoryFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Prefix { get; set; }
        public StringFilter Description { get; set; }
        public IdFilter ParentId { get; set; }
        public StringFilter Path { get; set; }
        public LongFilter Level { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter ImageId { get; set; }
        public GuidFilter RowId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<CategoryFilter> OrFilter { get; set; }
        public CategoryOrder OrderBy {get; set;}
        public CategorySelect Selects {get; set;}
        public string Search { get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CategoryOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
        Prefix = 3,
        Description = 4,
        Parent = 5,
        Path = 6,
        Level = 7,
        HasChildren = 8,
        Status = 9,
        Image = 10,
        Row = 14,
        Used = 15,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum CategorySelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
        Prefix = E._3,
        Description = E._4,
        Parent = E._5,
        Path = E._6,
        Level = E._7,
        HasChildren = E._8,
        Status = E._9,
        Image = E._10,
        Row = E._14,
        Used = E._15,
    }
}
