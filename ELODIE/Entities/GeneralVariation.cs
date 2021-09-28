using System;
using System.Collections.Generic;
using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ELODIE.Entities
{
    public class GeneralVariation : DataEntity,  IEquatable<GeneralVariation>
    {
        public long Id { get; set; }
        public long GeneralVariationGroupingId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public bool Used { get; set; }
        public Guid RowId { get; set; }
        public GeneralVariationGrouping GeneralVariationGrouping { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public bool Equals(GeneralVariation other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class GeneralVariationFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public IdFilter GeneralVariationGroupingId { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter StatusId { get; set; }
        public GuidFilter RowId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<GeneralVariationFilter> OrFilter { get; set; }
        public GeneralVariationOrder OrderBy {get; set;}
        public GeneralVariationSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GeneralVariationOrder
    {
        Id = 0,
        GeneralVariationGrouping = 1,
        Code = 2,
        Name = 3,
        Status = 4,
        Used = 5,
        Row = 9,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum GeneralVariationSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        GeneralVariationGrouping = E._1,
        Code = E._2,
        Name = E._3,
        Status = E._4,
        Used = E._5,
        Row = E._9,
    }
}
