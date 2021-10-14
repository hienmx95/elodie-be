using ELODIE.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using ELODIE.Entities;

namespace ELODIE.Rpc.customer_sales_order
{
    public class CustomerSalesOrder_CustomerDTO : DataDTO
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
        

        public CustomerSalesOrder_CustomerDTO() {}
        public CustomerSalesOrder_CustomerDTO(Customer Customer)
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
            
            this.Errors = Customer.Errors;
        }
    }

    public class CustomerSalesOrder_CustomerFilterDTO : FilterDTO
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
        
        public CustomerOrder OrderBy { get; set; }
    }
}