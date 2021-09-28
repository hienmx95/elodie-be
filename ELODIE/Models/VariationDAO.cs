using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class VariationDAO
    {
        public VariationDAO()
        {
            ItemVariationMappings = new HashSet<ItemVariationMappingDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long VariationGroupingId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }
        public bool Used { get; set; }

        public virtual VariationGroupingDAO VariationGrouping { get; set; }
        public virtual ICollection<ItemVariationMappingDAO> ItemVariationMappings { get; set; }
    }
}
