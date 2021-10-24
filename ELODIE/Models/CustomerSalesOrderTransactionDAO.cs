using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class CustomerSalesOrderTransactionDAO
    {
        public long Id { get; set; }
        public long CustomerSalesOrderId { get; set; }
        public long OrganizationId { get; set; }
        public long SalesEmployeeId { get; set; }
        public long ItemId { get; set; }
        public long UnitOfMeasureId { get; set; }
        public long Quantity { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Revenue { get; set; }
        public long TypeId { get; set; }
        public DateTime OrderDate { get; set; }

        public virtual CustomerSalesOrderDAO CustomerSalesOrder { get; set; }
        public virtual ItemDAO Item { get; set; }
        public virtual OrganizationDAO Organization { get; set; }
        public virtual AppUserDAO SalesEmployee { get; set; }
        public virtual TransactionTypeDAO Type { get; set; }
        public virtual UnitOfMeasureDAO UnitOfMeasure { get; set; }
    }
}
