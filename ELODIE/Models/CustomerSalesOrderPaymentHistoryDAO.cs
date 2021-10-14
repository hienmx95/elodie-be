using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class CustomerSalesOrderPaymentHistoryDAO
    {
        public long Id { get; set; }
        public long CustomerSalesOrderId { get; set; }
        public string PaymentMilestone { get; set; }
        public decimal? PaymentPercentage { get; set; }
        public decimal? PaymentAmount { get; set; }
        public long? PaymentTypeId { get; set; }
        public string Description { get; set; }
        public bool? IsPaid { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual CustomerSalesOrderDAO CustomerSalesOrder { get; set; }
        public virtual PaymentTypeDAO PaymentType { get; set; }
    }
}
