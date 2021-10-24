using ELODIE.Common;
using ELODIE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELODIE.Rpc.reports.report_customer_sales_order.report_customer_sales_order_by_item
{
    public class ReportCustomerSalesOrderByItem_ProductGroupingDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }
        public string Path { get; set; }
        public ReportCustomerSalesOrderByItem_ProductGroupingDTO() { }
        public ReportCustomerSalesOrderByItem_ProductGroupingDTO(ProductGrouping ProductGrouping)
        {
            this.Id = ProductGrouping.Id;
            this.Code = ProductGrouping.Code;
            this.Name = ProductGrouping.Name;
            this.ParentId = ProductGrouping.ParentId;
            this.Path = ProductGrouping.Path;
            this.Errors = ProductGrouping.Errors;
        }
    }

    public class ReportCustomerSalesOrderByItem_ProductGroupingFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter ParentId { get; set; }
        public StringFilter Path { get; set; }
        public ProductGroupingOrder OrderBy { get; set; }
    }
}
