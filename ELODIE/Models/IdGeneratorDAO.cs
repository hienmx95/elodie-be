using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class IdGeneratorDAO
    {
        public long Id { get; set; }
        public long IdGeneratorTypeId { get; set; }
        public bool Used { get; set; }
        public long Counter { get; set; }
    }
}
