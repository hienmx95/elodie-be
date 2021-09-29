using ELODIE.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace ELODIE.Entities
{
    public class AppUser : DataEntity, IEquatable<AppUser>
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string OtpCode { get; set; }
        public DateTime? OtpExpired { get; set; }
        public string Token { get; set; }
        public string DisplayName { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public DateTime? Birthday { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Department { get; set; }
        public long OrganizationId { get; set; }
        public long SexId { get; set; }
        public long StatusId { get; set; }
        public Guid RowId { get; set; }
        public bool Used { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Organization Organization { get; set; }
        public Sex Sex { get; set; }
        public Status Status { get; set; }
        public List<AppUserRoleMapping> AppUserRoleMappings { get; set; }
        public List<AppUserSiteMapping> AppUserSiteMappings { get; set; }
        public bool Equals(AppUser other)
        {
            return other != null && Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class AppUserFilter : FilterEntity
    {
        public IdFilter Id { get; set; }
        public StringFilter Username { get; set; }
        public StringFilter Password { get; set; }
        public StringFilter DisplayName { get; set; }
        public StringFilter Address { get; set; }
        public StringFilter Email { get; set; }
        public StringFilter Phone { get; set; }
        public StringFilter Department { get; set; }
        public IdFilter OrganizationId { get; set; }
        public IdFilter SexId { get; set; }
        public IdFilter StatusId { get; set; }
        public DateFilter Birthday { get; set; }
        public IdFilter RoleId { get; set; }
        public List<AppUserFilter> OrFilter { get; set; }
        public AppUserOrder OrderBy { get; set; }
        public AppUserSelect Selects { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AppUserOrder
    {
        Id = 0,
        Username = 1,
        Password = 2,
        DisplayName = 3,
        Address = 4,
        Email = 5,
        Phone = 6,
        Department = 8,
        Organization = 9,
        Sex = 10,
        Status = 11,
        Birthday = 12,
    }

    [Flags]
    public enum AppUserSelect : long
    {
        ALL = E.ALL,
        Id = E._0,
        Username = E._1,
        Password = E._2,
        DisplayName = E._3,
        Address = E._4,
        Email = E._5,
        Phone = E._6,
        Department = E._8,
        Organization = E._9,
        Sex = E._10,
        Status = E._11,
        Birthday = E._12,
        RowId = E._13,
        Avatar = E._14,
    }
}
