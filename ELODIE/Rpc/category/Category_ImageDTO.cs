using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.category
{
    public class Category_ImageDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public string Url { get; set; }
        
        public string ThumbnailUrl { get; set; }
        
        public Guid? RowId { get; set; }
        

        public Category_ImageDTO() {}
        public Category_ImageDTO(Image Image)
        {
            
            this.Id = Image.Id;
            
            this.Name = Image.Name;
            
            this.Url = Image.Url;
            
            this.ThumbnailUrl = Image.ThumbnailUrl;
            
            this.Errors = Image.Errors;
        }
    }

    public class Category_ImageFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Name { get; set; }
        
        public StringFilter Url { get; set; }
        
        public StringFilter ThumbnailUrl { get; set; }
        
        public GuidFilter RowId { get; set; }
        
        public ImageOrder OrderBy { get; set; }
    }
}