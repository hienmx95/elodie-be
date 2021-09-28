using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class ItemDAO
    {
        public ItemDAO()
        {
            ItemHistories = new HashSet<ItemHistoryDAO>();
            ItemImageMappings = new HashSet<ItemImageMappingDAO>();
            ItemVariationMappings = new HashSet<ItemVariationMappingDAO>();
        }

        public long Id { get; set; }
        public long ProductId { get; set; }
        public string Code { get; set; }
        public string ERPCode { get; set; }
        public string Name { get; set; }
        public string ScanCode { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool Used { get; set; }
        public Guid RowId { get; set; }

        public virtual ProductDAO Product { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<ItemHistoryDAO> ItemHistories { get; set; }
        public virtual ICollection<ItemImageMappingDAO> ItemImageMappings { get; set; }
        public virtual ICollection<ItemVariationMappingDAO> ItemVariationMappings { get; set; }
    }
}
