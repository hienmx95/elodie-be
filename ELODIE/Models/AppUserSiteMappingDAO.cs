using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class AppUserSiteMappingDAO
    {
        public long AppUserId { get; set; }
        public long SiteId { get; set; }
        public bool Enabled { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
        public virtual SiteDAO Site { get; set; }
    }
}
