using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class EditedPriceStatusDAO
    {
        public EditedPriceStatusDAO()
        {
            CustomerSalesOrderContents = new HashSet<CustomerSalesOrderContentDAO>();
            CustomerSalesOrders = new HashSet<CustomerSalesOrderDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<CustomerSalesOrderContentDAO> CustomerSalesOrderContents { get; set; }
        public virtual ICollection<CustomerSalesOrderDAO> CustomerSalesOrders { get; set; }
    }
}
