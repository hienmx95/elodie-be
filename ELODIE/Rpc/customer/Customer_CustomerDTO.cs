using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.customer
{
    public class Customer_CustomerDTO : DataDTO
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
        public Customer_AppUserDTO AppUser { get; set; }
        public Customer_CodeGeneratorRuleDTO CodeGeneratorRule { get; set; }
        public Customer_AppUserDTO Creator { get; set; }
        public Customer_CustomerSourceDTO CustomerSource { get; set; }
        public Customer_DistrictDTO District { get; set; }
        public Customer_NationDTO Nation { get; set; }
        public Customer_OrganizationDTO Organization { get; set; }
        public Customer_ProfessionDTO Profession { get; set; }
        public Customer_ProvinceDTO Province { get; set; }
        public Customer_SexDTO Sex { get; set; }
        public Customer_StatusDTO Status { get; set; }
        public Customer_WardDTO Ward { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Customer_CustomerDTO() {}
        public Customer_CustomerDTO(Customer Customer)
        {
            this.Id = Customer.Id;
            this.Code = Customer.Code;
            this.CodeDraft = Customer.CodeDraft;
            this.Name = Customer.Name;
            this.Phone = Customer.Phone;
            this.Address = Customer.Address;
            this.NationId = Customer.NationId;
            this.ProvinceId = Customer.ProvinceId;
            this.DistrictId = Customer.DistrictId;
            this.WardId = Customer.WardId;
            this.Birthday = Customer.Birthday;
            this.Email = Customer.Email;
            this.ProfessionId = Customer.ProfessionId;
            this.CustomerSourceId = Customer.CustomerSourceId;
            this.SexId = Customer.SexId;
            this.StatusId = Customer.StatusId;
            this.AppUserId = Customer.AppUserId;
            this.CreatorId = Customer.CreatorId;
            this.OrganizationId = Customer.OrganizationId;
            this.Used = Customer.Used;
            this.RowId = Customer.RowId;
            this.CodeGeneratorRuleId = Customer.CodeGeneratorRuleId;
            this.AppUser = Customer.AppUser == null ? null : new Customer_AppUserDTO(Customer.AppUser);
            this.CodeGeneratorRule = Customer.CodeGeneratorRule == null ? null : new Customer_CodeGeneratorRuleDTO(Customer.CodeGeneratorRule);
            this.Creator = Customer.Creator == null ? null : new Customer_AppUserDTO(Customer.Creator);
            this.CustomerSource = Customer.CustomerSource == null ? null : new Customer_CustomerSourceDTO(Customer.CustomerSource);
            this.District = Customer.District == null ? null : new Customer_DistrictDTO(Customer.District);
            this.Nation = Customer.Nation == null ? null : new Customer_NationDTO(Customer.Nation);
            this.Organization = Customer.Organization == null ? null : new Customer_OrganizationDTO(Customer.Organization);
            this.Profession = Customer.Profession == null ? null : new Customer_ProfessionDTO(Customer.Profession);
            this.Province = Customer.Province == null ? null : new Customer_ProvinceDTO(Customer.Province);
            this.Sex = Customer.Sex == null ? null : new Customer_SexDTO(Customer.Sex);
            this.Status = Customer.Status == null ? null : new Customer_StatusDTO(Customer.Status);
            this.Ward = Customer.Ward == null ? null : new Customer_WardDTO(Customer.Ward);
            this.CreatedAt = Customer.CreatedAt;
            this.UpdatedAt = Customer.UpdatedAt;
            this.Errors = Customer.Errors;
        }
    }

    public class Customer_CustomerFilterDTO : FilterDTO
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
        public CustomerOrder OrderBy { get; set; }
    }
}
