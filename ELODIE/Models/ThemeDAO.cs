using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class ThemeDAO
    {
        public ThemeDAO()
        {
            Sites = new HashSet<SiteDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<SiteDAO> Sites { get; set; }
    }
}
