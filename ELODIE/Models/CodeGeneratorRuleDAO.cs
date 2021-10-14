using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class CodeGeneratorRuleDAO
    {
        public CodeGeneratorRuleDAO()
        {
            CodeGeneratorRuleEntityComponentMappings = new HashSet<CodeGeneratorRuleEntityComponentMappingDAO>();
            CustomerSalesOrders = new HashSet<CustomerSalesOrderDAO>();
            Customers = new HashSet<CustomerDAO>();
            Products = new HashSet<ProductDAO>();
        }

        public long Id { get; set; }
        public long EntityTypeId { get; set; }
        public long AutoNumberLenth { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid RowId { get; set; }
        public bool Used { get; set; }

        public virtual EntityTypeDAO EntityType { get; set; }
        public virtual StatusDAO Status { get; set; }
        public virtual ICollection<CodeGeneratorRuleEntityComponentMappingDAO> CodeGeneratorRuleEntityComponentMappings { get; set; }
        public virtual ICollection<CustomerSalesOrderDAO> CustomerSalesOrders { get; set; }
        public virtual ICollection<CustomerDAO> Customers { get; set; }
        public virtual ICollection<ProductDAO> Products { get; set; }
    }
}
