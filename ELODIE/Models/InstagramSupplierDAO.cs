using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class InstagramSupplierDAO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long CountView { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
