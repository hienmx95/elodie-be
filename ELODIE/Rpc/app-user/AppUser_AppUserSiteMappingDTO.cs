using ELODIE.Common;
using ELODIE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Rpc.app_user
{
    public class AppUser_AppUserSiteMappingDTO : DataDTO
    {
        public long AppUserId { get; set; }
        public long SiteId { get; set; }
        public bool Enabled { get; set; }
        public AppUser_AppUserDTO AppUser { get; set; }
        public AppUser_SiteDTO Site { get; set; }
        public AppUser_AppUserSiteMappingDTO() { }
        public AppUser_AppUserSiteMappingDTO(AppUserSiteMapping AppUserSiteMapping)
        {
            this.AppUserId = AppUserSiteMapping.AppUserId;
            this.SiteId = AppUserSiteMapping.SiteId;
            this.Enabled = AppUserSiteMapping.Enabled;
            this.AppUser = AppUserSiteMapping.AppUser == null ? null : new AppUser_AppUserDTO(AppUserSiteMapping.AppUser);
            this.Site = AppUserSiteMapping.Site == null ? null : new AppUser_SiteDTO(AppUserSiteMapping.Site);
        }
    }
}
