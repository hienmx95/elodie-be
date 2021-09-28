using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class SiteDAO
    {
        public SiteDAO()
        {
            AppUserSiteMappings = new HashSet<AppUserSiteMappingDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Logo { get; set; }
        public bool IsDisplay { get; set; }
        public long? ThemeId { get; set; }
        public Guid RowId { get; set; }

        public virtual ThemeDAO Theme { get; set; }
        public virtual ICollection<AppUserSiteMappingDAO> AppUserSiteMappings { get; set; }
    }
}
