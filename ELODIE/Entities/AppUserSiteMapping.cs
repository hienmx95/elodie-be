using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Entities
{
    public class AppUserSiteMapping : DataEntity
    {
        public long AppUserId { get; set; }
        public long SiteId { get; set; }
        public bool Enabled { get; set; }
        public AppUser AppUser { get; set; }
        public Site Site { get; set; }
        public bool Equals(AppUserSiteMapping other)
        {
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class AppUserSiteMappingFilter : FilterEntity 
    {
        public IdFilter AppUserId { get; set; }
        public IdFilter SiteId { get; set; }
        public List<AppUserSiteMappingFilter> OrFilter { get; set; }
        public AppUserSiteMappingFilterOrder OrderBy { get; set; }
        public AppUserSiteMappingFilterSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AppUserSiteMappingFilterOrder
    {
        AppUser = 0,
        Site = 1,
    }

    [Flags]
    public enum AppUserSiteMappingFilterSelect : long
    {
        ALL = E.ALL,
        AppUser = E._0,
        Site = E._1,
    }
}
