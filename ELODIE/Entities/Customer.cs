using System;
using System.Collections.Generic;
using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ELODIE.Entities
{
    public class Customer : DataEntity,  IEquatable<Customer>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string CodeDraft { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public long? NationId { get; set; }
        public long? ProvinceId { get; set; }
        public long? DistrictId { get; set; }
        public long? WardId { get; set; }
        public DateTime? Birthday { get; set; }
        public string Email { get; set; }
        public long? ProfessionId { get; set; }
        public long? CustomerSourceId { get; set; }
        public long? SexId { get; set; }
        public long StatusId { get; set; }
        public long AppUserId { get; set; }
        public long CreatorId { get; set; }
        public long OrganizationId { get; set; }
        public bool Used { get; set; }
        public Guid RowId { get; set; }
        public long? CodeGeneratorRuleId { get; set; }
        public AppUser AppUser { get; set; }
        public CodeGeneratorRule CodeGeneratorRule { get; set; }
        public AppUser Creator { get; set; }
        public CustomerSource CustomerSource { get; set; }
        public District District { get; set; }
        public Nation Nation { get; set; }
        public Organization Organization { get; set; }
        public Profession Profession { get; set; }
        public Province Province { get; set; }
        public Sex Sex { get; set; }
        public Status Status { get; set; }
        public Ward Ward { get; set; }
        public List<CustomerCustomerGroupingMapping> CustomerCustomerGroupingMappings { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(Customer other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Code != other.Code) return false;
            if (this.CodeDraft != other.CodeDraft) return false;
            if (this.Name != other.Name) return false;
            if (this.Phone != other.Phone) return false;
            if (this.Address != other.Address) return false;
            if (this.NationId != other.NationId) return false;
            if (this.ProvinceId != other.ProvinceId) return false;
            if (this.DistrictId != other.DistrictId) return false;
            if (this.WardId != other.WardId) return false;
            if (this.Birthday != other.Birthday) return false;
            if (this.Email != other.Email) return false;
            if (this.ProfessionId != other.ProfessionId) return false;
            if (this.CustomerSourceId != other.CustomerSourceId) return false;
            if (this.SexId != other.SexId) return false;
            if (this.StatusId != other.StatusId) return false;
            if (this.AppUserId != other.AppUserId) return false;
            if (this.CreatorId != other.CreatorId) return false;
            if (this.OrganizationId != other.OrganizationId) return false;
            if (this.Used != other.Used) return false;
            if (this.RowId != other.RowId) return false;
            if (this.CodeGeneratorRuleId != other.CodeGeneratorRuleId) return false;
            if (this.CustomerCustomerGroupingMappings?.Count != other.CustomerCustomerGroupingMappings?.Count) return false;
            else if (this.CustomerCustomerGroupingMappings != null && other.CustomerCustomerGroupingMappings != null)
            {
                for (int i = 0; i < CustomerCustomerGroupingMappings.Count; i++)
                {
                    CustomerCustomerGroupingMapping CustomerCustomerGroupingMapping = CustomerCustomerGroupingMappings[i];
                    CustomerCustomerGroupingMapping otherCustomerCustomerGroupingMapping = other.CustomerCustomerGroupingMappings[i];
                    if (CustomerCustomerGroupingMapping == null && otherCustomerCustomerGroupingMapping != null)
                        return false;
                    if (CustomerCustomerGroupingMapping != null && otherCustomerCustomerGroupingMapping == null)
                        return false;
                    if (CustomerCustomerGroupingMapping.Equals(otherCustomerCustomerGroupingMapping) == false)
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

    public class CustomerFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter CodeDraft { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter Phone { get; set; }
        public StringFilter Address { get; set; }
        public IdFilter NationId { get; set; }
        public IdFilter ProvinceId { get; set; }
        public IdFilter DistrictId { get; set; }
        public IdFilter WardId { get; set; }
        public DateFilter Birthday { get; set; }
        public StringFilter Email { get; set; }
        public IdFilter ProfessionId { get; set; }
        public IdFilter CustomerSourceId { get; set; }
        public IdFilter SexId { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter AppUserId { get; set; }
        public IdFilter CreatorId { get; set; }
        public IdFilter OrganizationId { get; set; }
        public GuidFilter RowId { get; set; }
        public IdFilter CodeGeneratorRuleId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<CustomerFilter> OrFilter { get; set; }
        public CustomerOrder OrderBy {get; set;}
        public CustomerSelect Selects {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CustomerOrder
    {
        Id = 0,
        Code = 1,
        CodeDraft = 2,
        Name = 3,
        Phone = 4,
        Address = 5,
        Nation = 6,
        Province = 7,
        District = 8,
        Ward = 9,
        Birthday = 10,
        Email = 11,
        Profession = 12,
        CustomerSource = 13,
        Sex = 14,
        Status = 15,
        AppUser = 16,
        Creator = 17,
        Organization = 18,
        Used = 22,
        Row = 23,
        CodeGeneratorRule = 24,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum CustomerSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        CodeDraft = E._2,
        Name = E._3,
        Phone = E._4,
        Address = E._5,
        Nation = E._6,
        Province = E._7,
        District = E._8,
        Ward = E._9,
        Birthday = E._10,
        Email = E._11,
        Profession = E._12,
        CustomerSource = E._13,
        Sex = E._14,
        Status = E._15,
        AppUser = E._16,
        Creator = E._17,
        Organization = E._18,
        Used = E._22,
        Row = E._23,
        CodeGeneratorRule = E._24,
    }
}
