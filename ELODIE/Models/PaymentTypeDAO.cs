using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class PaymentTypeDAO
    {
        public PaymentTypeDAO()
        {
            CustomerSalesOrderPaymentHistories = new HashSet<CustomerSalesOrderPaymentHistoryDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool Used { get; set; }
        public Guid RowId { get; set; }

        public virtual ICollection<CustomerSalesOrderPaymentHistoryDAO> CustomerSalesOrderPaymentHistories { get; set; }
    }
}
