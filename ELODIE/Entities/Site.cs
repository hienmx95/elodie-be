using System;
using System.Collections.Generic;
using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ELODIE.Entities
{
    public class Site : DataEntity,  IEquatable<Site>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Logo { get; set; }
        public long? ThemeId { get; set; }
        public bool IsDisplay { get; set; }
        public Guid RowId { get; set; }
        public Theme Theme { get; set; }

        public bool Equals(Site other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class SiteFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Description { get; set; }
        public bool? IsDisplay { get; set; }
        public SiteOrder OrderBy {get; set;}
        public SiteSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SiteOrder
    {
        Id = 0,
        Name = 1,
        Code = 2,
    }

    [Flags]
    public enum SiteSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Name = E._1,
        Code = E._2,
    }
}
