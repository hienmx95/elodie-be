using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class ItemVariationMappingDAO
    {
        public long ItemId { get; set; }
        public long VariationId { get; set; }

        public virtual ItemDAO Item { get; set; }
        public virtual VariationDAO Variation { get; set; }
    }
}
