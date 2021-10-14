using System;
using System.Collections.Generic;
using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ELODIE.Entities
{
    public class OrderSource : DataEntity,  IEquatable<OrderSource>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long? Priority { get; set; }
        public string Description { get; set; }
        public long StatusId { get; set; }
        public bool Used { get; set; }
        public Guid RowId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(OrderSource other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Code != other.Code) return false;
            if (this.Name != other.Name) return false;
            if (this.Priority != other.Priority) return false;
            if (this.Description != other.Description) return false;
            if (this.StatusId != other.StatusId) return false;
            if (this.Used != other.Used) return false;
            if (this.RowId != other.RowId) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class OrderSourceFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public LongFilter Priority { get; set; }
        public StringFilter Description { get; set; }
        public IdFilter StatusId { get; set; }
        public GuidFilter RowId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<OrderSourceFilter> OrFilter { get; set; }
        public OrderSourceOrder OrderBy {get; set;}
        public OrderSourceSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrderSourceOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
        Priority = 3,
        Description = 4,
        Status = 5,
        Used = 9,
        Row = 10,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum OrderSourceSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
        Priority = E._3,
        Description = E._4,
        Status = E._5,
        Used = E._9,
        Row = E._10,
    }
}
