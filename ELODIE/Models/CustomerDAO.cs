using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class CustomerDAO
    {
        public CustomerDAO()
        {
            CustomerCustomerGroupingMappings = new HashSet<CustomerCustomerGroupingMappingDAO>();
            CustomerSalesOrders = new HashSet<CustomerSalesOrderDAO>();
        }

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
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool Used { get; set; }
        public Guid RowId { get; set; }
        public long? CodeGeneratorRuleId { get; set; }

        public virtual AppUserDAO AppUser { get; set; }
        public virtual CodeGeneratorRuleDAO CodeGeneratorRule { get; set; }
        public virtual AppUserDAO Creator { get; set; }
        public virtual CustomerSourceDAO CustomerSource { get; set; }
        public virtual DistrictDAO District { get; set; }
        public virtual NationDAO Nation { get; set; }
        public virtual OrganizationDAO Organization { get; set; }
        public virtual ProfessionDAO Profession { get; set; }
        public virtual ProvinceDAO Province { get; set; }
        public virtual SexDAO Sex { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual WardDAO Ward { get; set; }
        public virtual ICollection<CustomerCustomerGroupingMappingDAO> CustomerCustomerGroupingMappings { get; set; }
        public virtual ICollection<CustomerSalesOrderDAO> CustomerSalesOrders { get; set; }
    }
}
