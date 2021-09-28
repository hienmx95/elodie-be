using System;
using System.Collections.Generic;
using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ELODIE.Entities
{
    public class Supplier : DataEntity,  IEquatable<Supplier>
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string TaxCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public string Address { get; set; }
        public long? NationId { get; set; }
        public long? ProvinceId { get; set; }
        public long? DistrictId { get; set; }
        public long? WardId { get; set; }
        public string OwnerName { get; set; }
        public long? PersonInChargeId { get; set; }
        public long StatusId { get; set; }
        public string Description { get; set; }
        public bool Used { get; set; }
        public Guid RowId { get; set; }
        public District District { get; set; }
        public Nation Nation { get; set; }
        public AppUser PersonInCharge { get; set; }
        public Province Province { get; set; }
        public Status Status { get; set; }
        public Ward Ward { get; set; }
        public List<SupplierBankAccount> SupplierBankAccounts { get; set; }
        public List<SupplierCategoryMapping> SupplierCategoryMappings { get; set; }
        public List<SupplierContactor> SupplierContactors { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public bool Equals(Supplier other)
        {
            if (other == null) return false;
            if (this.Id != other.Id) return false;
            if (this.Code != other.Code) return false;
            if (this.Name != other.Name) return false;
            if (this.TaxCode != other.TaxCode) return false;
            if (this.Phone != other.Phone) return false;
            if (this.Email != other.Email) return false;
            if (this.Avatar != other.Avatar) return false;
            if (this.Address != other.Address) return false;
            if (this.NationId != other.NationId) return false;
            if (this.ProvinceId != other.ProvinceId) return false;
            if (this.DistrictId != other.DistrictId) return false;
            if (this.WardId != other.WardId) return false;
            if (this.OwnerName != other.OwnerName) return false;
            if (this.PersonInChargeId != other.PersonInChargeId) return false;
            if (this.StatusId != other.StatusId) return false;
            if (this.Description != other.Description) return false;
            if (this.Used != other.Used) return false;
            if (this.RowId != other.RowId) return false;
            if (this.SupplierBankAccounts?.Count != other.SupplierBankAccounts?.Count) return false;
            else if (this.SupplierBankAccounts != null && other.SupplierBankAccounts != null)
            {
                for (int i = 0; i < SupplierBankAccounts.Count; i++)
                {
                    SupplierBankAccount SupplierBankAccount = SupplierBankAccounts[i];
                    SupplierBankAccount otherSupplierBankAccount = other.SupplierBankAccounts[i];
                    if (SupplierBankAccount == null && otherSupplierBankAccount != null)
                        return false;
                    if (SupplierBankAccount != null && otherSupplierBankAccount == null)
                        return false;
                    if (SupplierBankAccount.Equals(otherSupplierBankAccount) == false)
                        return false;
                }
            }
            if (this.SupplierCategoryMappings?.Count != other.SupplierCategoryMappings?.Count) return false;
            else if (this.SupplierCategoryMappings != null && other.SupplierCategoryMappings != null)
            {
                for (int i = 0; i < SupplierCategoryMappings.Count; i++)
                {
                    SupplierCategoryMapping SupplierCategoryMapping = SupplierCategoryMappings[i];
                    SupplierCategoryMapping otherSupplierCategoryMapping = other.SupplierCategoryMappings[i];
                    if (SupplierCategoryMapping == null && otherSupplierCategoryMapping != null)
                        return false;
                    if (SupplierCategoryMapping != null && otherSupplierCategoryMapping == null)
                        return false;
                    if (SupplierCategoryMapping.Equals(otherSupplierCategoryMapping) == false)
                        return false;
                }
            }
            if (this.SupplierContactors?.Count != other.SupplierContactors?.Count) return false;
            else if (this.SupplierContactors != null && other.SupplierContactors != null)
            {
                for (int i = 0; i < SupplierContactors.Count; i++)
                {
                    SupplierContactor SupplierContactor = SupplierContactors[i];
                    SupplierContactor otherSupplierContactor = other.SupplierContactors[i];
                    if (SupplierContactor == null && otherSupplierContactor != null)
                        return false;
                    if (SupplierContactor != null && otherSupplierContactor == null)
                        return false;
                    if (SupplierContactor.Equals(otherSupplierContactor) == false)
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

    public class SupplierFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public StringFilter TaxCode { get; set; }
        public StringFilter Phone { get; set; }
        public StringFilter Email { get; set; }
        public StringFilter Avatar { get; set; }
        public StringFilter Address { get; set; }
        public IdFilter NationId { get; set; }
        public IdFilter ProvinceId { get; set; }
        public IdFilter DistrictId { get; set; }
        public IdFilter WardId { get; set; }
        public StringFilter OwnerName { get; set; }
        public IdFilter PersonInChargeId { get; set; }
        public IdFilter StatusId { get; set; }
        public StringFilter Description { get; set; }
        public GuidFilter RowId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public List<SupplierFilter> OrFilter { get; set; }
        public SupplierOrder OrderBy {get; set;}
        public SupplierSelect Selects {get; set;}
        public string Search {get; set;}
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SupplierOrder
    {
        Id = 0,
        Code = 1,
        Name = 2,
        TaxCode = 3,
        Phone = 4,
        Email = 5,
        Avatar = 6,
        Address = 7,
        Nation = 8,
        Province = 9,
        District = 10,
        Ward = 11,
        OwnerName = 12,
        PersonInCharge = 13,
        Status = 14,
        Description = 15,
        Used = 19,
        Row = 20,
        CreatedAt = 50,
        UpdatedAt = 51,
    }

    [Flags]
    public enum SupplierSelect:long
    {
        ALL = E.ALL,
        Id = E._0,
        Code = E._1,
        Name = E._2,
        TaxCode = E._3,
        Phone = E._4,
        Email = E._5,
        Avatar = E._6,
        Address = E._7,
        Nation = E._8,
        Province = E._9,
        District = E._10,
        Ward = E._11,
        OwnerName = E._12,
        PersonInCharge = E._13,
        Status = E._14,
        Description = E._15,
        Used = E._19,
        Row = E._20,
    }
}
