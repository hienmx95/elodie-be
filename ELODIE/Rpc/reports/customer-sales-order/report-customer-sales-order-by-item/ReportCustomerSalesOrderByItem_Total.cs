using ELODIE.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Rpc.reports.report_customer_sales_order.report_customer_sales_order_by_item
{
    public class ReportCustomerSalesOrderByItem_TotalDTO : DataDTO
    {
        public long TotalSalesStock { get; set; }
        public long TotalPromotionStock { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalRevenue { get; set; }
    }

}
