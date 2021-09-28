using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.category
{
    public class Category_CategoryDTO : DataDTO
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
        public Category_ImageDTO Image { get; set; }
        public Category_CategoryDTO Parent { get; set; }
        public Category_StatusDTO Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Category_CategoryDTO() {}
        public Category_CategoryDTO(Category Category)
        {
            this.Id = Category.Id;
            this.Code = Category.Code;
            this.Name = Category.Name;
            this.Prefix = Category.Prefix;
            this.Description = Category.Description;
            this.ParentId = Category.ParentId;
            this.Path = Category.Path;
            this.Level = Category.Level;
            this.HasChildren = Category.HasChildren;
            this.StatusId = Category.StatusId;
            this.ImageId = Category.ImageId;
            this.RowId = Category.RowId;
            this.Used = Category.Used;
            this.Image = Category.Image == null ? null : new Category_ImageDTO(Category.Image);
            this.Parent = Category.Parent == null ? null : new Category_CategoryDTO(Category.Parent);
            this.Status = Category.Status == null ? null : new Category_StatusDTO(Category.Status);
            this.CreatedAt = Category.CreatedAt;
            this.UpdatedAt = Category.UpdatedAt;
            this.Errors = Category.Errors;
        }
    }

    public class Category_CategoryFilterDTO : FilterDTO
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
        public CategoryOrder OrderBy { get; set; }
        public string Search { get; set; }
    }
}
