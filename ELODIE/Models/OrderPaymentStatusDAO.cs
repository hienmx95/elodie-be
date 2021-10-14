using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class OrderPaymentStatusDAO
    {
        public OrderPaymentStatusDAO()
        {
            CustomerSalesOrders = new HashSet<CustomerSalesOrderDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<CustomerSalesOrderDAO> CustomerSalesOrders { get; set; }
    }
}
