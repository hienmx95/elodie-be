using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.supplier
{
    public class Supplier_CategoryDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        
        public long? ParentId { get; set; }
        
        public string Path { get; set; }
        
        public long Level { get; set; }

        public bool HasChildren { get; set; }
        
        public long StatusId { get; set; }
        
        public long? ImageId { get; set; }
        
        public Guid RowId { get; set; }
        
        public bool Used { get; set; }
        

        public Supplier_CategoryDTO() {}
        public Supplier_CategoryDTO(Category Category)
        {
            
            this.Id = Category.Id;
            
            this.Code = Category.Code;
            
            this.Name = Category.Name;
            
            this.ParentId = Category.ParentId;
            
            this.Path = Category.Path;
            
            this.Level = Category.Level;

            this.HasChildren = Category.HasChildren;
            
            this.StatusId = Category.StatusId;
            
            this.ImageId = Category.ImageId;
            
            this.RowId = Category.RowId;
            
            this.Used = Category.Used;
            
            this.Errors = Category.Errors;
        }
    }

    public class Supplier_CategoryFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public IdFilter ParentId { get; set; }
        
        public StringFilter Path { get; set; }
        
        public LongFilter Level { get; set; }
        
        public IdFilter StatusId { get; set; }
        
        public IdFilter ImageId { get; set; }
        
        public GuidFilter RowId { get; set; }
        
        public CategoryOrder OrderBy { get; set; }
    }
}