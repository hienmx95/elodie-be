using System;
using System.Collections.Generic;
using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ELODIE.Entities
{
    public class Warehouse : DataEntity,  IEquatable<Warehouse>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public long OrganizationId { get; set; }
        public long? ProvinceId { get; set; }
        public long? DistrictId { get; set; }
        public long? WardId { get; set; }
        public long StatusId { get; set; }
        public Guid RowId { get; set; }
        public District District { get; set; }
        public Organization Organization { get; set; }
        public Province Province { get; set; }
        public Status Status { get; set; }
        public Ward Ward { get; set; }
        public List<Inventory> Inventories { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(Warehouse other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Code != other.Code) return false;
            if (this.Name != other.Name) return false;
            if (this.Address != other.Address) return false;
            if (this.OrganizationId != other.OrganizationId) return false;
            if (this.ProvinceId != other.ProvinceId) return false;
            if (this.DistrictId != other.DistrictId) return false;
            if (this.WardId != other.WardId) return false;
            if (this.StatusId != other.StatusId) return false;
            if (this.RowId != other.RowId) return false;
            if (this.Inventories?.Count != other.Inventories?.Count) return false;
            else if (this.Inventories != null && other.Inventories != null)
            {
                for (int i = 0; i < Inventories.Count; i++)
                {
                    Inventory Inventory = Inventories[i];
                    Inventory otherInventory = other.Inventories[i];
                    if (Inventory == null && otherInventory != null)
                        return false;
                    if (Inventory != null && otherInventory == null)
                        return false;
                    if (Inventory.Equals(otherInventory) == false)
                        return false;
                }
            }
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class WarehouseFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Address { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter ProvinceId { get; set; }
        public IdFilter DistrictId { get; set; }
        public IdFilter WardId { get; set; }
        public IdFilter StatusId { get; set; }
        public GuidFilter RowId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<WarehouseFilter> OrFilter { get; set; }
        public WarehouseOrder OrderBy {get; set;}
        public WarehouseSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum WarehouseOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
        Address = 3,
        Organization = 4,
        Province = 5,
        District = 6,
        Ward = 7,
        Status = 8,
        Row = 12,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum WarehouseSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
        Address = E._3,
        Organization = E._4,
        Province = E._5,
        District = E._6,
        Ward = E._7,
        Status = E._8,
        Row = E._12,
    }
}
