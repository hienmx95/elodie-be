using System;
using System.Collections.Generic;
using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ELODIE.Entities
{
    public class EditedPriceStatus : DataEntity,  IEquatable<EditedPriceStatus>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        
        public bool Equals(EditedPriceStatus other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Code != other.Code) return false;
            if (this.Name != other.Name) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class EditedPriceStatusFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public List<EditedPriceStatusFilter> OrFilter { get; set; }
        public EditedPriceStatusOrder OrderBy {get; set;}
        public EditedPriceStatusSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EditedPriceStatusOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
    }

    [Flags]
    public enum EditedPriceStatusSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
    }
}
