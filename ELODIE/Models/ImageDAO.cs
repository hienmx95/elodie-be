using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class ImageDAO
    {
        public ImageDAO()
        {
            Categories = new HashSet<CategoryDAO>();
            ItemImageMappings = new HashSet<ItemImageMappingDAO>();
            ProductImageMappings = new HashSet<ProductImageMappingDAO>();
        }

        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Tên
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Đường dẫn Url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public DateTime UpdatedAt { get; set; }
        /// <summary>
        /// Ngày xoá
        /// </summary>
        public DateTime? DeletedAt { get; set; }
        /// <summary>
        /// Đường dẫn Url
        /// </summary>
        public string ThumbnailUrl { get; set; }
        public Guid RowId { get; set; }

        public virtual ICollection<CategoryDAO> Categories { get; set; }
        public virtual ICollection<ItemImageMappingDAO> ItemImageMappings { get; set; }
        public virtual ICollection<ProductImageMappingDAO> ProductImageMappings { get; set; }
    }
}
