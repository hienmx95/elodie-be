using System;
using System.Collections.Generic;

namespace ELODIE.Models
{
    public partial class TransactionTypeDAO
    {
        public TransactionTypeDAO()
        {
            CustomerSalesOrderTransactions = new HashSet<CustomerSalesOrderTransactionDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<CustomerSalesOrderTransactionDAO> CustomerSalesOrderTransactions { get; set; }
    }
}
