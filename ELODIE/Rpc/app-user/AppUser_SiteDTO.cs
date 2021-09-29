using ELODIE.Common;
using ELODIE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Rpc.app_user
{
    public class AppUser_SiteDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Logo { get; set; }
        public bool IsDisplay { get; set; }
        public AppUser_SiteDTO() { }
        public AppUser_SiteDTO(Site Site)
        {
            this.Id = Site.Id;
            this.Code = Site.Code;
            this.Name = Site.Name;
            this.Description = Site.Description;
            this.Icon = Site.Icon;
            this.Logo = Site.Logo;
            this.IsDisplay = Site.IsDisplay;
        }
    }
}
