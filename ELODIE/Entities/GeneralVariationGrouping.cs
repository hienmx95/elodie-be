using System;
using System.Collections.Generic;
using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ELODIE.Entities
{
    public class GeneralVariationGrouping : DataEntity,  IEquatable<GeneralVariationGrouping>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public bool Used { get; set; }
        public Guid RowId { get; set; }
        public Status Status { get; set; }
        public List<GeneralVariation> GeneralVariations { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public bool Equals(GeneralVariationGrouping other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class GeneralVariationGroupingFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter StatusId { get; set; }
        public GuidFilter RowId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<GeneralVariationGroupingFilter> OrFilter { get; set; }
        public GeneralVariationGroupingOrder OrderBy {get; set;}
        public GeneralVariationGroupingSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GeneralVariationGroupingOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
        Status = 3,
        Used = 4,
        Row = 8,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum GeneralVariationGroupingSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
        Status = E._3,
        Used = E._4,
        Row = E._8,
    }
}
